﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;
using Perfon.Interfaces.Notifications;
using Perfon.Interfaces.PerfCounters;

namespace Perfon.Core.Notifications
{
    public abstract class ThresholdBaseNotification : IThresholdNotification
    {
        public ThresholdBaseNotification(float thresholdValue, string message = "")
        {
            ThresholdValue = thresholdValue;
            Message = message;

            if(string.IsNullOrEmpty(Message))
            {
                Message = "Violated theshould " + ThresholdValue+" :";
            }
        }

        public float ThresholdValue { get; private set; }

        public bool IsThresholdViolated { get; protected set; }

        public abstract bool TestThresholdOk(IPerformanceCounter counter);

        public event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolated;

        public event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolationRecovered;

        protected void RaiseThresholdViolated(ThreshouldNotificationEventArg arg)
        {
            if (OnThresholdViolated != null)
            {
                OnThresholdViolated(new object(), arg);
            }
        }
        protected void RaiseThresholdViolationRecovered(ThreshouldNotificationEventArg arg)
        {
            if (OnThresholdViolationRecovered != null)
            {
                OnThresholdViolationRecovered(new object(), arg);
            }
        }

        public string Message { get; protected set; }
    }
}
