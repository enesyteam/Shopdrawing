using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.Project.ServiceExtensions.Messaging
{
	public static class MessagingServiceExtensions
	{
		public static void DisplayFailureMessage(this IServiceProvider source, string failure, string description)
		{
			IMessageDisplayService messageDisplayService = source.MessageDisplayService();
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string dialogFailedMessage = StringTable.DialogFailedMessage;
			object[] objArray = new object[] { failure, description };
			messageDisplayService.ShowError(string.Format(currentCulture, dialogFailedMessage, objArray));
		}

		public static bool PromptUserYesNo(this IServiceProvider source, string text)
		{
			MessageBoxArgs messageBoxArg = new MessageBoxArgs()
			{
				Message = text,
				Button = MessageBoxButton.YesNo,
				Image = MessageBoxImage.Exclamation
			};
			return source.MessageDisplayService().ShowMessage(messageBoxArg) == MessageBoxResult.Yes;
		}

		public static MessageBoxResult ShowSuppressibleWarning(this IServiceProvider source, MessageBoxArgs args, string optionIdentifier, MessageBoxResult suppressedResult)
		{
			return source.ShowSuppressibleWarning(args, optionIdentifier, suppressedResult, source.ProjectConfigurationObject());
		}

		public static MessageBoxResult ShowSuppressibleWarning(this IServiceProvider source, MessageBoxArgs args, string optionIdentifier, MessageBoxResult suppressedResult, IConfigurationObject configObject)
		{
			bool flag = (configObject != null ? (bool)configObject.GetProperty(optionIdentifier, false) : false);
			if (flag)
			{
				return suppressedResult;
			}
			MessageBoxResult messageBoxResult = source.MessageDisplayService().ShowMessage(args, out flag);
			if (flag && configObject != null)
			{
				configObject.SetProperty(optionIdentifier, true);
			}
			return messageBoxResult;
		}
	}
}