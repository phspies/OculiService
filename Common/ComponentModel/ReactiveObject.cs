using OculiService.Common;
using OculiService.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace DoubleTake.ComponentModel
{
    public abstract class ReactiveObject : IReactiveObject, INotifyPropertyChanged, IDisposable
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly Subject<PropertyChangedEventInfo> propertyChangedSubject = new Subject<PropertyChangedEventInfo>();
        private readonly Dispatcher dispatcher;
        private bool isDisposed;

        public Dispatcher Dispatcher
        {
            get
            {
                this.EnsureNotDisposed();
                return this.dispatcher;
            }
        }

        public IObservable<PropertyChangedEventInfo> PropertyChangedEvents
        {
            get
            {
                this.EnsureNotDisposed();
                return this.propertyChangedSubject.AsObservable<PropertyChangedEventInfo>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected ReactiveObject()
        {
            this.dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            this.RegisterDisposable((IDisposable)this.propertyChangedSubject);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckAccess()
        {
            this.EnsureNotDisposed();
            if (this.dispatcher != null)
                return this.dispatcher.CheckAccess();
            return true;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;
            this.OnDispose();
            this.isDisposed = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void VerifyAccess()
        {
            this.EnsureNotDisposed();
            if (this.dispatcher == null)
                return;
            this.dispatcher.VerifyAccess();
        }

        protected void EnsureNotDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        protected virtual void OnDispose()
        {
            this.propertyChangedSubject.Dispose();
            this.disposables.Dispose();
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventInfo info)
        {
            Invariant.ArgumentNotNull((object)info, "info");
            this.propertyChangedSubject.OnNext(info);
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged((object)this, new PropertyChangedEventArgs(info.PropertyName));
        }

        protected void RegisterDisposable(IDisposable disposable)
        {
            Invariant.ArgumentNotNull((object)disposable, "disposable");
            this.VerifyAccess();
            this.disposables.Add(disposable);
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            Invariant.ArgumentNotNull((object)propertyName, "propertyName");
            this.VerifyAccess();
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;
            PropertyChangedEventInfo<T> changedEventInfo = new PropertyChangedEventInfo<T>((object)this, propertyName, backingField, value);
            backingField = value;
            this.OnPropertyChanged((PropertyChangedEventInfo)changedEventInfo);
            return true;
        }

        private sealed class ReactiveValue<T> : IReactiveValue<T>, IObservable<T>, IDisposable
        {
            private readonly Subject<T> source;
            private readonly IDisposable subscription;
            private bool isDisposed;
            private T value;

            public T Value
            {
                get
                {
                    this.EnsureNotDisposed();
                    return this.value;
                }
            }


            public void Dispose()
            {
                if (this.isDisposed)
                    return;
                this.subscription.Dispose();
                this.source.Dispose();
                this.isDisposed = true;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                this.EnsureNotDisposed();
                return this.source.Subscribe(observer);
            }

            private void EnsureNotDisposed()
            {
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}

