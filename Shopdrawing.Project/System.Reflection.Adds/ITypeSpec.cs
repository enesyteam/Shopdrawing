namespace System.Reflection.Adds
{
	internal interface ITypeSpec : ITypeSignatureBlob, ITypeProxy
	{
		Token TypeSpecToken
		{
			get;
		}
	}
}