using System.Collections.Generic;

namespace Motion_lie_detection
{
    public abstract class SuitController
    {
        protected List<Observer> observers;

        /// <summary>
        /// Connect to suit and return if connecting was succesfull
        /// </summary>
        public abstract bool Connect();

        public abstract bool Disconnect();

        /// <summary>
        /// Start suit calibration and return if succeful
        /// </summary>
        public abstract bool Calibrate();

        public abstract bool Register(Observer observer);
    }
}
