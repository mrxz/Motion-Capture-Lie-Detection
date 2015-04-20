using System;

namespace Motion_lie_detection
{
	/**
	 * Simple observer interface.
	 */
    public interface Observer
    {

		/**
		 * Method that is invoked upon an observable event.
		 * @param data The data corresponding with the notification.
		 */
		void notify (Object data);

    }
}
