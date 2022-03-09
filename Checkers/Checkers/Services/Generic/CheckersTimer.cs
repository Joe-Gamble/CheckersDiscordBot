// <copyright file="CheckersTimer.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

namespace Checkers.Services.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CheckersTimer : IDisposable
    {
        private readonly TimerCallback realCallback;
        private readonly Timer timer;
        private TimeSpan period;
        private DateTime next;

        public CheckersTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            this.timer = new Timer(Callback, state, dueTime, period);
            this.realCallback = callback;
            this.period = period;
            this.next = DateTime.Now.Add(dueTime);
        }

        private void Callback(object state)
        {
            this.next = DateTime.Now.Add(this.period);
            this.realCallback(state);
        }

        public TimeSpan Period => period;

        public DateTime Next => next;

        public TimeSpan DueTime => next - DateTime.Now;

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            this.period = period;
            next = DateTime.Now.Add(dueTime);
            return timer.Change(dueTime, period);
        }

        public void Dispose() => timer.Dispose();
    }
}
