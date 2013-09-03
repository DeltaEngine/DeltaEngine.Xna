using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Content.Online
{
	/// <summary>
	/// Connects to the Content Service and can reload files at runtime, but only works when a
	/// developer API Key has been setup. Will not be used for the end user.
	/// </summary>
	public class DeveloperOnlineContentLoader : ContentLoader
	{
		//ncrunch: no coverage start
		public DeveloperOnlineContentLoader(OnlineServiceConnection connection,
			string editorContentPath = null)
			: base(editorContentPath ?? "Content")
		{
			this.connection = connection;
			isIgnoringSetProject = false;
			connection.loadContentMetaData += OnLoadContentMetaData;
			connection.DataReceived += OnDataReceived;
		}

		protected readonly OnlineServiceConnection connection;
		protected bool isIgnoringSetProject;

		public void OnLoadContentMetaData()
		{
			RefreshMetaData();
			isContentReady = true;
		}

		protected void RefreshMetaData()
		{
			metaData.Clear();
			ParseXmlNode(XmlFile.Root);
		}

		private XmlFile XmlFile
		{
			get
			{
				if (file != null)
					return file;
				if (File.Exists(ContentMetaDataFilePath))
					file = new XmlFile(ContentMetaDataFilePath);
				else
				{
					var root = new XmlData("ContentMetaData");
					root.AddAttribute("Name", ProjectName);
					root.AddAttribute("Type", "Scene");
					root.AddAttribute("LastTimeUpdated", DateTime.Now);
					root.AddAttribute("ContentDeviceName", "Delta");
					file = new XmlFile(root);
				}
				return file;
			}
		}
		private XmlFile file;

		private void ParseXmlNode(XmlData currentNode)
		{
			lock (XmlFile)
			{
				var currentElement = ParseContentMetaData(currentNode.Attributes);
				var name = currentNode.GetAttributeValue("Name");
				if (!metaData.ContainsKey(name) && currentNode.Parent != null)
					metaData.Add(name, currentElement);
				foreach (var node in currentNode.Children)
					ParseXmlNode(node);
			}
		}

		private static ContentMetaData ParseContentMetaData(List<XmlAttribute> attributes)
		{
			var data = new ContentMetaData();
			foreach (var attribute in attributes)
				switch (attribute.Name)
				{
				case "Name":
					data.Name = attribute.Value;
					break;
				case "Type":
					data.Type = attribute.Value.TryParse(ContentType.Image);
					break;
				case "LastTimeUpdated":
					data.LastTimeUpdated = DateExtensions.Parse(attribute.Value);
					break;
				case "LocalFilePath":
					data.LocalFilePath = attribute.Value;
					break;
				case "PlatformFileId":
					data.PlatformFileId = attribute.Value.Convert<int>();
					break;
				case "FileSize":
					data.FileSize = attribute.Value.Convert<int>();
					break;
				default:
					data.Values.Add(attribute.Name, attribute.Value);
					break;
				}
			if (string.IsNullOrEmpty(data.Name))
				throw new InvalidContentMetaDataNameIsAlwaysNeeded(attributes.ToText());
			return data;
		}

		public class InvalidContentMetaDataNameIsAlwaysNeeded : Exception
		{
			public InvalidContentMetaDataNameIsAlwaysNeeded(string message)
				: base(message) {}
		}

		protected readonly Dictionary<string, ContentMetaData> metaData =
			new Dictionary<string, ContentMetaData>(StringComparer.OrdinalIgnoreCase);

		private bool isContentReady;

		private void OnDataReceived(object message)
		{
			var newProject = message as SetProject;
			var receivedFile = message as UpdateContent;
			var deletedFile = message as DeleteContent;
			if (newProject != null && !isIgnoringSetProject)
				VerifyProjectAndGetContent(newProject);
			else if (receivedFile != null)
				UpdateLocalContent(receivedFile);
			else if (deletedFile != null)
				DeleteLocalContent(deletedFile);
		}

		protected void VerifyProjectAndGetContent(SetProject newProject)
		{
			if (newProject.Permissions == ProjectPermissions.None)
				throw new NoPermissionToUseProject(newProject.ProjectName);
			ProjectName = newProject.ProjectName;
			RemoveContentDirectoryIfFromOtherProject();
			connection.Send(new CheckProjectContent(GetCurrentContentMetaData()));
		}

		private void RemoveContentDirectoryIfFromOtherProject()
		{
			var contentMetaDataProjectName = XmlFile.Root.GetAttributeValue("Name");
			if (contentMetaDataProjectName == ProjectName)
				return;
			file = null;
			if (Directory.Exists(contentPath))
				Directory.Delete(contentPath, true);
		}

		private string GetCurrentContentMetaData()
		{
			return File.Exists(ContentMetaDataFilePath)
				? File.ReadAllText(ContentMetaDataFilePath) : XmlFile.Root.ToXmlString();
		}

		private class NoPermissionToUseProject : Exception
		{
			public NoPermissionToUseProject(string projectName)
				: base(projectName) {}
		}

		protected string ProjectName { get; set; }

		private void UpdateLocalContent(UpdateContent content)
		{
			if (!Directory.Exists(contentPath))
				Directory.CreateDirectory(contentPath);
			lock (XmlFile)
			{
				UpdateContentMetaDataFile(content.MetaData, XmlFile.Root);
				XmlFile.Save(ContentMetaDataFilePath);
				foreach (var contentFile in content.Files)
					if (contentFile.name != null)
						File.WriteAllBytes(Path.Combine(contentPath, contentFile.name), contentFile.data);
			}
			if (ContentChanged != null)
				ContentChanged();
		}

		private static void UpdateContentMetaDataFile(ContentMetaData entry, XmlData parent)
		{
			RemoveExistingEntry(entry, parent);
			var child = new XmlData("ContentMetaData");
			parent.AddChild(child);
			child.Attributes.Clear();
			child.Attributes.AddRange(ParseAttributes(entry));
		}

		private static void RemoveExistingEntry(ContentMetaData entry, XmlData parent)
		{
			foreach (var child in parent.Children)
				if (child.GetAttributeValue("Name") == entry.Name)
				{
					parent.RemoveChild(child);
					break;
				}
		}

		private static IEnumerable<XmlAttribute> ParseAttributes(ContentMetaData entry)
		{
			var attributes = new List<XmlAttribute>();
			attributes.Add(new XmlAttribute("Name", entry.Name));
			attributes.Add(new XmlAttribute("Type", entry.Type));
			attributes.Add(new XmlAttribute("LastTimeUpdated", entry.LastTimeUpdated));
			if (entry.Language != null)
				attributes.Add(new XmlAttribute("Language", entry.Language));
			if (!string.IsNullOrEmpty(entry.LocalFilePath))
				attributes.Add(new XmlAttribute("LocalFilePath", entry.LocalFilePath));
			if (entry.PlatformFileId != 0)
				attributes.Add(new XmlAttribute("PlatformFileId", entry.PlatformFileId));
			if (entry.FileSize != 0)
				attributes.Add(new XmlAttribute("FileSize", entry.FileSize));
			attributes.AddRange(entry.Values.Select(value => new XmlAttribute(value.Key, value.Value)));
			return attributes;
		}

		public event Action ContentChanged;

		private void DeleteLocalContent(DeleteContent content)
		{
			lock (XmlFile)
			{
				XmlData entryToDelete = null;
				foreach (var child in XmlFile.Root.Children)
					if (child.GetAttributeValue("Name") == content.ContentName)
						entryToDelete = child;
				if (entryToDelete == null)
					return;
				DeleteFiles(entryToDelete);
				XmlFile.Root.RemoveChild(entryToDelete);
				XmlFile.Save(ContentMetaDataFilePath);
			}
			if (ContentChanged != null)
				ContentChanged();
		}

		private void DeleteFiles(XmlData entry)
		{
			var localFilePath = entry.GetAttributeValue("LocalFilePath");
			if (string.IsNullOrEmpty(localFilePath))
				foreach (var child in entry.Children)
					DeleteFiles(child);
			else
				File.Delete(Path.Combine(contentPath, localFilePath));
		}

		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			if (isContentReady)
				return true;
			isContentReady = File.Exists(ContentMetaDataFilePath);
			if (isContentReady)
				ParseXmlNode(XmlFile.Root);
			return isContentReady;
		}

		protected override ContentMetaData GetMetaData(string contentName,
			Type contentClassType = null)
		{
			if (!isContentReady)
				throw new ContentNotReady();
			return metaData.ContainsKey(contentName) ? metaData[contentName] : null;
		}

		public class ContentNotReady : Exception {}

		public override void Dispose()
		{
			// ReSharper disable DelegateSubtraction
			connection.loadContentMetaData -= OnLoadContentMetaData;
			connection.DataReceived -= OnDataReceived;
			base.Dispose();
		}
	}
}