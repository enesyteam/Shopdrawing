using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class DocumentTypeManager : IDocumentTypeManager
	{
		private DocumentTypeManager.DocumentTypeCollection documentTypes;

		public IDocumentTypeCollection DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public DocumentTypeManager(IDocumentType unknownDocumentType)
		{
			this.documentTypes = new DocumentTypeManager.DocumentTypeCollection(unknownDocumentType);
		}

		public void Register(IDocumentType documentType)
		{
			if (documentType == null)
			{
				throw new ArgumentNullException("documentType");
			}
			this.documentTypes.Add(documentType);
		}

		public void Unregister(IDocumentType documentType)
		{
			this.documentTypes.Remove(documentType);
		}

		private class DocumentTypeCollection : IDocumentTypeCollection, IEnumerable<IDocumentType>, IEnumerable
		{
			private Dictionary<string, IDocumentType> dictionary;

			private IDocumentType unknownDocumentType;

			public int Count
			{
				get
				{
					return this.dictionary.Count;
				}
			}

			public IDocumentType this[string name]
			{
				get
				{
					IDocumentType documentType = null;
					this.dictionary.TryGetValue(name, out documentType);
					if (documentType != null)
					{
						return documentType;
					}
					return this.unknownDocumentType;
				}
			}

			public IDocumentType UnknownDocumentType
			{
				get
				{
					return this.unknownDocumentType;
				}
			}

			public DocumentTypeCollection(IDocumentType unknownDocumentType)
			{
				this.unknownDocumentType = unknownDocumentType;
			}

			public void Add(IDocumentType documentType)
			{
				this.dictionary.Add(documentType.Name, documentType);
			}

			public bool Contains(IDocumentType documentType)
			{
				return this.dictionary.ContainsValue(documentType);
			}

			public IEnumerator<IDocumentType> GetEnumerator()
			{
				return this.dictionary.Values.GetEnumerator();
			}

			public void Remove(IDocumentType documentType)
			{
				if (this.dictionary.ContainsKey(documentType.Name))
				{
					this.dictionary.Remove(documentType.Name);
				}
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.dictionary.Values.GetEnumerator();
			}
		}
	}
}