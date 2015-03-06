using System.Collections.Generic;

namespace Motion_lie_detection
{
    public abstract class SuitController
    {
		protected readonly List<Observer> observers;

		public SuitController() {
			observers = new List<Observer> ();
		}

		/**
		 * Attempt to connect to the suit.
		 * @return True if connecting was succesfull, false otherwise.
		 */
        public abstract bool Connect();
		/**
		 * Disconnect the suit.
		 * @return True if disconnection was successful, false otherwise.
		 */
        public abstract bool Disconnect();

		/**
		 * Start calibrating the suit.
		 * @return True if the calibration is started, false otherwise.
		 */
        public abstract bool Calibrate();

		public void Register(Observer observer) {
			this.observers.Add (observer);
		}
    }
}
