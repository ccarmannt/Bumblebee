﻿using System;
using System.Drawing.Imaging;
using System.IO;

using Bumblebee.Extensions;
using Bumblebee.Interfaces;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Bumblebee.Setup
{
	public class Session
	{
		public virtual ISettings Settings { get; private set; }

		public virtual IWebDriver Driver { get; private set; }

		public virtual IMonkey Monkey { get; protected set; }

		public Session(IDriverEnvironment environment) : this(environment, new Settings())
		{
		}

		public Session(IDriverEnvironment environment, ISettings settings)
		{
			Settings = settings;
			Driver = environment.CreateWebDriver();
		}

		public virtual TBlock NavigateTo<TBlock>(string url) where TBlock : IBlock
		{
			Driver.Navigate().GoToUrl(url);
			return CurrentBlock<TBlock>();
		}

		public virtual TBlock CurrentBlock<TBlock>(IWebElement tag = null) where TBlock : IBlock
		{
			return Factory.CreateBlockFromSession<TBlock>(this);
		}

		public virtual void End()
		{
			if (Driver != null)
			{
				Driver.Quit();

				Driver.Dispose();

				Driver = null;
			}
		}

		public virtual Session CaptureScreen()
		{
			var filename = String.Format("{0}.png", CallStack.GetCallingMethod().GetFullName());
			var path = Path.Combine(Settings.ScreenCapturePath, filename);
			return CaptureScreen(path);
		}

		public virtual Session CaptureScreen(string path)
		{
			var screenshot = Driver.TakeScreenshot();

			var extension = Path.GetExtension(path);

			if (String.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
			{
				screenshot.SaveAsFile(path, ImageFormat.Png);
			}
			else if ((String.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase))
					|| (String.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)))
			{
				screenshot.SaveAsFile(path, ImageFormat.Jpeg);
			}
			else if (String.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase))
			{
				screenshot.SaveAsFile(path, ImageFormat.Bmp);
			}
			else if (String.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
			{
				screenshot.SaveAsFile(path, ImageFormat.Gif);
			}
			else
			{
				throw new ArgumentException("Unable to determine image format. The supported formats are BMP, GIF, JPEG and PNG.", "path");
			}

			return this;
		}
	}

	public class Session<TDriverEnvironment> : Session
		where TDriverEnvironment : IDriverEnvironment, new()
	{
		public Session() : base(new TDriverEnvironment())
		{
		}

		public Session(ISettings settings) : base(new TDriverEnvironment(), settings)
		{
			
		}
	}
}