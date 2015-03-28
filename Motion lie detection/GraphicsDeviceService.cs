using System;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;


// Copyright (C) Microsoft Corporation. All rights reserved.


#pragma warning disable 67

namespace Motion_lie_detection
{
	public class GraphicsDeviceService : IGraphicsDeviceService
	{
		static GraphicsDeviceService instance;
		static int refCount;


		public GraphicsDevice GraphicsDevice
		{
			get { return graphicsDevice; }
		}

		GraphicsDevice graphicsDevice;

		PresentationParameters parameters;




		public event EventHandler<EventArgs> DeviceCreated, DeviceDisposing, DeviceReset, DeviceResetting;


		GraphicsDeviceService(IntPtr windowHandle, int width, int height)
		{
			parameters = new PresentationParameters ();

			parameters.BackBufferWidth = Math.Max (width, 1);
			parameters.BackBufferHeight = Math.Max (height, 1);
			parameters.BackBufferFormat = SurfaceFormat.Color;
			parameters.DepthStencilFormat = DepthFormat.Depth24;
			parameters.DeviceWindowHandle = windowHandle;
			parameters.PresentationInterval = PresentInterval.Immediate;
			parameters.IsFullScreen = false;

			graphicsDevice = new GraphicsDevice (GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, parameters);
		}




		public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
		{
			if (Interlocked.Increment (ref refCount) == 1) {
				instance = new GraphicsDeviceService (windowHandle, width, height);
			}
			return instance;
		}

		public void Release(bool disposing)
		{
			if (Interlocked.Decrement (ref refCount) == 0) {
				if (disposing) {
					// call callback
					if (DeviceDisposing != null) {
						DeviceDisposing (this, EventArgs.Empty);
					}
					graphicsDevice.Dispose ();
				}
				graphicsDevice = null;
			}
		}

		/// <summary>
		/// Resets the graphics device to whichever is bigger out of the specified
		/// resolution or its current size. This behavior means the device will
		/// demand-grow to the largest of all its GraphicsDeviceControl clients.
		/// </summary>
		public void ResetDevice(int width, int height)
		{
			throw new NotImplementedException ();
		}


	}
		
}

