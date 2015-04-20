using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Abstract class for suit controllers.
	 */
    public abstract class SuitController
    {
		/**
		 * List of observers that should be notified when a new frame is ready.
		 */
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

		public void Register(Observer observer) 
		{
			observers.Add (observer);
		}

		public void NotifyObservers(Object data) 
		{
			foreach (Observer observer in observers)
				observer.notify (data);
		}
    }
}
