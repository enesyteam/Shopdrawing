// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.WavefrontObjAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class WavefrontObjAsset : AssetDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Obj.png";
      }
    }

    public override string Name
    {
      get
      {
        return "ObjModel";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.WavefrontObjAssetDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.WavefrontObjAssetFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".obj"
        };
      }
    }

    public WavefrontObjAsset(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    protected override SceneElement CreateElement(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, string url)
    {
      IMessageDisplayService messageDisplayService = this.DesignerContext.MessageDisplayService;
      try
      {
        Uri uri = viewModel.XamlDocument.DocumentContext.MakeDesignTimeUri(new Uri(url, UriKind.RelativeOrAbsolute));
        string directoryName = Path.GetDirectoryName(uri.LocalPath);
        using (StreamReader streamReader = new StreamReader(uri.LocalPath))
        {
          try
          {
            IMessageLoggingService messageLoggingService = this.DesignerContext.MessageLoggingService;
            messageLoggingService.Clear();
            ModelVisual3D modelVisual3D = new WavefrontObjAsset.WavefrontObjLoader(viewModel, messageLoggingService, messageDisplayService).LoadObjFile(streamReader, directoryName, Path.GetFileNameWithoutExtension(url));
            ModelVisual3DElement modelVisual3DElement = (ModelVisual3DElement) viewModel.CreateSceneNode((object) modelVisual3D);
            WavefrontObjAsset.WalkModelVisualTreeAssigningDesignTimeBounds(modelVisual3DElement, modelVisual3D);
            return (SceneElement) modelVisual3DElement;
          }
          catch (InvalidOperationException ex)
          {
            string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjAssetCreateElementFailedDialogMessage, new object[2]
            {
              (object) url,
              (object) ex.Message
            });
            messageDisplayService.ShowError(message);
            return (SceneElement) null;
          }
        }
      }
      catch (IOException ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjAssetCreateElementFailedDialogMessage, new object[2]
        {
          (object) url,
          (object) ex.Message
        });
        messageDisplayService.ShowError(message);
        return (SceneElement) null;
      }
    }

    protected override bool DoesNodeReferenceUrl(DocumentNode node, string url)
    {
      return false;
    }

    protected override void RefreshInstance(SceneElement element, SceneDocument sceneDocument, string url)
    {
    }

    private static void WalkModelVisualTreeAssigningDesignTimeBounds(ModelVisual3DElement modelVisual3DElement, ModelVisual3D modelVisual3D)
    {
      for (int index = 0; index < modelVisual3DElement.Children.Count; ++index)
        WavefrontObjAsset.WalkModelVisualTreeAssigningDesignTimeBounds((ModelVisual3DElement) modelVisual3DElement.Children[index], (ModelVisual3D) modelVisual3D.Children[index]);
      if (modelVisual3DElement.Model3DContent == null)
        return;
      modelVisual3DElement.Model3DContent.DesignTimeBounds = modelVisual3D.Content.Bounds;
    }

    public override bool CanAddToProject(IProject project)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null || !JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.ModelVisual3D))
        return false;
      return base.CanAddToProject(project);
    }

    private class WavefrontObjLoader
    {
      private static readonly WavefrontObjAsset.WavefrontObjLoader.KeywordData[] Keywords = new WavefrontObjAsset.WavefrontObjLoader.KeywordData[63]
      {
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("v", WavefrontObjAsset.WavefrontObjLoader.KeywordType.GeometricVertex),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("vn", WavefrontObjAsset.WavefrontObjLoader.KeywordType.VertexNormal),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("vt", WavefrontObjAsset.WavefrontObjLoader.KeywordType.TextureVertex),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("f", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Face),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("fo", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Face),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("g", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Group),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("l", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Line),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("o", WavefrontObjAsset.WavefrontObjLoader.KeywordType.ObjectName),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("s", WavefrontObjAsset.WavefrontObjLoader.KeywordType.SmoothingGroup),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("mtllib", WavefrontObjAsset.WavefrontObjLoader.KeywordType.MaterialLibrary),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("usemtl", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UseMaterial),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("usemap", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UseMap),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("bevel", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("bmat", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("bsp", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("bzp", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("c_interop", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("cdc", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("con", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("cstype", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("ctech", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("curv", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("curv2", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("d_interop", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("deg", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("end", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("hole", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("lod", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("maplib", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("mg", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("p", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("param", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("parm", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("res", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("scrv", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("shadow_obj", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("sp", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("stech", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("step", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("surf", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("trace_obj", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("trim", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("vp", WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("newmtl", WavefrontObjAsset.WavefrontObjLoader.KeywordType.NewMaterial),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("illum", WavefrontObjAsset.WavefrontObjLoader.KeywordType.IlluminationMode),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Ka", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Ambient),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Kd", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Diffuse),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Ks", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Specular),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Ke", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Emissive),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("d", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Alpha),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Tf", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Transparency),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Tr", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Alpha),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Ns", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Shininess),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("Ni", WavefrontObjAsset.WavefrontObjLoader.KeywordType.RefractionIndex),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_Ka", WavefrontObjAsset.WavefrontObjLoader.KeywordType.AmbientTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_bump", WavefrontObjAsset.WavefrontObjLoader.KeywordType.BumpTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("bump", WavefrontObjAsset.WavefrontObjLoader.KeywordType.BumpTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_Kd", WavefrontObjAsset.WavefrontObjLoader.KeywordType.DiffuseTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_Ks", WavefrontObjAsset.WavefrontObjLoader.KeywordType.SpecularTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_Ns", WavefrontObjAsset.WavefrontObjLoader.KeywordType.ShininessTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("map_refl", WavefrontObjAsset.WavefrontObjLoader.KeywordType.ReflectionTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("refl", WavefrontObjAsset.WavefrontObjLoader.KeywordType.ReflectionTextureFilename),
        new WavefrontObjAsset.WavefrontObjLoader.KeywordData("sharpness", WavefrontObjAsset.WavefrontObjLoader.KeywordType.Sharpness)
      };
      private static readonly double GeneratedVertexTolerance = 1E-06;
      private static readonly Material DefaultWhiteMaterial = (Material) new DiffuseMaterial((Brush) new SolidColorBrush(Colors.White));
      private static readonly char[] Separators = new char[2]
      {
        ' ',
        '\t'
      };
      private static readonly char[] CommentCharacters = new char[2]
      {
        '#',
        '$'
      };
      private bool generateNormals = true;
      private bool generateTextureCoordinates = true;
      private SceneViewModel sceneViewModel;
      private IMessageLoggingService logger;
      private IMessageDisplayService manager;
      private int totalVerticesInObj;
      private int totalVertices;
      private int totalGeneratedVertices;
      private List<Point3D> coordinates;
      private List<Vector3D> vertexNormals;
      private List<Point> textureCoordinates;
      private Dictionary<int, List<int>> smoothingGroups;
      private List<WavefrontObjAsset.WavefrontObjLoader.Face> faces;
      private List<WavefrontObjAsset.WavefrontObjLoader.Group> groups;
      private int nextFaceIndex;

      public WavefrontObjLoader(SceneViewModel sceneViewModel, IMessageLoggingService logger, IMessageDisplayService manager)
      {
        this.sceneViewModel = sceneViewModel;
        this.logger = logger;
        this.manager = manager;
      }

      public ModelVisual3D LoadObjFile(StreamReader streamReader, string rootPath, string filename)
      {
        this.coordinates = new List<Point3D>();
        this.vertexNormals = new List<Vector3D>();
        this.textureCoordinates = new List<Point>();
        this.smoothingGroups = new Dictionary<int, List<int>>();
        this.faces = new List<WavefrontObjAsset.WavefrontObjLoader.Face>();
        this.groups = new List<WavefrontObjAsset.WavefrontObjLoader.Group>();
        this.nextFaceIndex = 0;
        this.totalVerticesInObj = 0;
        this.totalVertices = 0;
        this.totalGeneratedVertices = 0;
        Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial> dictionary1 = (Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial>) null;
        bool flag = false;
        string name1 = "DefaultGroup";
        string name2 = (string) null;
        string materialName = "DefaultMaterial";
        int result = 0;
        Material material = WavefrontObjAsset.WavefrontObjLoader.DefaultMaterial();
        WavefrontObjAsset.WavefrontObjLoader.Group group = new WavefrontObjAsset.WavefrontObjLoader.Group(name1);
        string str1 = string.Empty;
        string str2;
        while ((str2 = streamReader.ReadLine()) != null)
        {
          int length = str2.IndexOfAny(WavefrontObjAsset.WavefrontObjLoader.CommentCharacters);
          if (length != -1)
            str2 = str2.Substring(0, length);
          int index1 = str2.IndexOfAny(WavefrontObjAsset.WavefrontObjLoader.Separators);
          string str3 = string.Empty;
          string index2 = string.Empty;
          if (str2.Trim().Length != 0)
          {
            string line;
            if (index1 != -1)
            {
              line = str2.Substring(0, index1);
              while (index1 < str2.Length && ((int) str2[index1] == 32 || (int) str2[index1] == 9))
                ++index1;
              index2 = str2.Substring(index1);
            }
            else
              line = str2;
            switch (WavefrontObjAsset.WavefrontObjLoader.ParseKeyword(line))
            {
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnrecognizedKeyword:
                this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorUnrecognizedKeyword, new object[1]
                {
                  (object) index2
                }));
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnimplementedKeyword:
                this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorUnimplementedKeyword, new object[1]
                {
                  (object) index2
                }));
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Face:
                WavefrontObjAsset.WavefrontObjLoader.Face face;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseFace(index2, out face) && this.NormalizeFaceIndices(face))
                {
                  this.faces.Add(face);
                  if (result != 0)
                  {
                    List<int> list;
                    if (!this.smoothingGroups.TryGetValue(result, out list))
                    {
                      list = new List<int>(20);
                      this.smoothingGroups.Add(result, list);
                    }
                    list.Add(this.faces.Count - 1);
                    continue;
                  }
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingFace);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Group:
                if (this.FinishCurrentGeometry(group, material, name2, materialName))
                  name2 = (string) null;
                this.FinishCurrentGroup(group);
                string str4 = index2.TrimEnd(WavefrontObjAsset.WavefrontObjLoader.Separators);
                if (str4.Length != 0)
                  name1 = str4;
                group = new WavefrontObjAsset.WavefrontObjLoader.Group(name1);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.MaterialLibrary:
                dictionary1 = this.LoadMtlFile(Path.Combine(rootPath, index2));
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.ObjectName:
                name2 = index2.TrimEnd(WavefrontObjAsset.WavefrontObjLoader.Separators);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.SmoothingGroup:
                string s = index2.Trim(WavefrontObjAsset.WavefrontObjLoader.Separators);
                if (s == "off")
                {
                  result = 0;
                  continue;
                }
                if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                {
                  this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorInvalidSmoothingGroup, new object[1]
                  {
                    (object) s
                  }));
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.UseMaterial:
                if (this.FinishCurrentGeometry(group, material, name2, materialName))
                  name2 = (string) null;
                if (dictionary1 == null && !flag)
                {
                  dictionary1 = this.LoadMtlFile(Path.Combine(rootPath, filename + ".mtl"));
                  flag = true;
                }
                if (dictionary1 != null)
                {
                  try
                  {
                    material = dictionary1[index2].Material;
                    materialName = index2;
                    continue;
                  }
                  catch (KeyNotFoundException ex)
                  {
                    this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorMaterialNotFound, new object[1]
                    {
                      (object) index2
                    }));
                    material = WavefrontObjAsset.WavefrontObjLoader.DefaultMaterial();
                    continue;
                  }
                }
                else
                {
                  this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorMaterialNotFound, new object[1]
                  {
                    (object) index2
                  }));
                  material = WavefrontObjAsset.WavefrontObjLoader.DefaultMaterial();
                  continue;
                }
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.GeometricVertex:
                Point3D vertex;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseVertex(index2, out vertex))
                {
                  ++this.totalVerticesInObj;
                  this.coordinates.Add(vertex);
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingVertex);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.VertexNormal:
                Vector3D vector;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseVertexNormal(index2, out vector))
                {
                  this.vertexNormals.Add(vector);
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingVertexNormal);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.TextureVertex:
                Point textureCoordinate;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseVertexTextureCoordinate(index2, out textureCoordinate))
                {
                  this.textureCoordinates.Add(textureCoordinate);
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingVertexTextureCoordinate);
                continue;
              default:
                continue;
            }
          }
        }
        this.FinishCurrentGeometry(group, material, name2, materialName);
        this.FinishCurrentGroup(group);
        if (this.generateNormals)
        {
          foreach (KeyValuePair<int, List<int>> keyValuePair1 in this.smoothingGroups)
          {
            List<int> list = keyValuePair1.Value;
            Dictionary<int, Vector3D> dictionary2 = new Dictionary<int, Vector3D>();
            for (int index1 = 0; index1 < list.Count; ++index1)
            {
              WavefrontObjAsset.WavefrontObjLoader.Face face = this.faces[list[index1]];
              if (face.NormalIndices == null)
              {
                for (int index2 = 0; index2 < face.CoordinateIndices.Count; ++index2)
                {
                  Vector3D normal;
                  if (!dictionary2.TryGetValue(face.CoordinateIndices[index2], out normal))
                  {
                    normal = face.Normal;
                    dictionary2.Add(face.CoordinateIndices[index2], normal);
                  }
                  else
                  {
                    normal += face.Normal;
                    dictionary2[face.CoordinateIndices[index2]] = normal;
                  }
                }
              }
            }
            Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
            foreach (KeyValuePair<int, Vector3D> keyValuePair2 in dictionary2)
            {
              Vector3D vector3D = keyValuePair2.Value;
              vector3D.Normalize();
              this.vertexNormals.Add(vector3D);
              dictionary3[keyValuePair2.Key] = this.vertexNormals.Count - 1;
            }
            for (int index1 = 0; index1 < list.Count; ++index1)
            {
              WavefrontObjAsset.WavefrontObjLoader.Face face = this.faces[list[index1]];
              if (face.NormalIndices == null)
              {
                face.NormalIndices = new List<int>(face.CoordinateIndices.Count);
                for (int index2 = 0; index2 < face.CoordinateIndices.Count; ++index2)
                  face.NormalIndices.Add(dictionary3[face.CoordinateIndices[index2]]);
              }
            }
          }
        }
        ModelVisual3D modelVisual3D = this.GenerateAvalonTree();
        this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderMessageTotalVerticesInFile, new object[1]
        {
          (object) this.totalVerticesInObj
        }));
        this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderMessageTotalVerticesInModel, new object[1]
        {
          (object) this.totalVertices
        }));
        this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderMessageVerticesGenerated, new object[1]
        {
          (object) this.totalGeneratedVertices
        }));
        return modelVisual3D;
      }

      private static Material DefaultMaterial()
      {
        Random random = new Random();
        Material material = (Material) new DiffuseMaterial((Brush) new SolidColorBrush(Color.FromScRgb(1f, (float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble())));
        return WavefrontObjAsset.WavefrontObjLoader.DefaultWhiteMaterial;
      }

      private Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial> LoadMtlFile(string mtlFilename)
      {
        Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial> materialLibrary = (Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial>) null;
        StreamReader streamReader;
        try
        {
          streamReader = new StreamReader(mtlFilename);
        }
        catch (FileNotFoundException ex)
        {
          this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorMaterialFileNotFound, new object[1]
          {
            (object) mtlFilename
          }));
          return materialLibrary;
        }
        using (streamReader)
        {
          try
          {
            this.ParseMtlFile(streamReader, Path.GetDirectoryName(mtlFilename), ref materialLibrary);
          }
          catch (InvalidOperationException ex)
          {
            this.manager.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjAssetCreateElementFailedDialogMessage, new object[2]
            {
              (object) mtlFilename,
              (object) ex.Message
            }));
          }
        }
        return materialLibrary;
      }

      private void ParseMtlFile(StreamReader streamReader, string rootPath, ref Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial> materialLibrary)
      {
        materialLibrary = new Dictionary<string, WavefrontObjAsset.WavefrontObjLoader.ObjMaterial>();
        WavefrontObjAsset.WavefrontObjLoader.ObjMaterial currentMaterial = new WavefrontObjAsset.WavefrontObjLoader.ObjMaterial();
        currentMaterial.Alpha = 1.0;
        string str1 = string.Empty;
        string str2;
        while ((str2 = streamReader.ReadLine()) != null)
        {
          string str3 = str2.TrimStart(WavefrontObjAsset.WavefrontObjLoader.Separators);
          int index = str3.IndexOfAny(WavefrontObjAsset.WavefrontObjLoader.Separators);
          string str4 = string.Empty;
          string str5 = string.Empty;
          if (str3.Trim().Length != 0)
          {
            string line;
            if (index != -1)
            {
              line = str3.Substring(0, index);
              while (index < str3.Length && ((int) str3[index] == 32 || (int) str3[index] == 9))
                ++index;
              str5 = str3.Substring(index);
            }
            else
              line = str3;
            switch (WavefrontObjAsset.WavefrontObjLoader.ParseKeyword(line))
            {
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnrecognizedKeyword:
                this.logger.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.WavefrontObjLoaderErrorIllegalMtlKeyword, new object[2]
                {
                  (object) line,
                  (object) str5
                }));
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.IlluminationMode:
                int result1;
                if (int.TryParse(str5, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
                {
                  currentMaterial.IlluminationMode = result1;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingIlluminationMode);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Alpha:
                double result2;
                if (double.TryParse(str5, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
                {
                  currentMaterial.Alpha = result2;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingAlpha);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Ambient:
                Color color1;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseColor(str5, out color1))
                {
                  currentMaterial.Ambient = color1;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingAmbientColor);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Diffuse:
                Color color2;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseColor(str5, out color2))
                {
                  currentMaterial.Diffuse = color2;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingDiffuseColor);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Specular:
                Color color3;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseColor(str5, out color3))
                {
                  currentMaterial.Specular = color3;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingSpecularColor);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Emissive:
                Color color4;
                if (WavefrontObjAsset.WavefrontObjLoader.ParseColor(str5, out color4))
                {
                  currentMaterial.Emissive = color4;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingEmissiveColor);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.AmbientTextureFilename:
                BitmapImage bitmapImage1 = this.LoadTexture(rootPath, str5);
                if (bitmapImage1 != null)
                {
                  currentMaterial.AmbientTextureFilename = str5;
                  currentMaterial.AmbientTexture = bitmapImage1;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.BumpTextureFilename:
                BitmapImage bitmapImage2 = this.LoadTexture(rootPath, str5);
                if (bitmapImage2 != null)
                {
                  currentMaterial.BumpTextureFilename = str5;
                  currentMaterial.BumpTexture = bitmapImage2;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.DiffuseTextureFilename:
                BitmapImage bitmapImage3 = this.LoadTexture(rootPath, str5);
                if (bitmapImage3 != null)
                {
                  currentMaterial.DiffuseTextureFilename = str5;
                  currentMaterial.DiffuseTexture = bitmapImage3;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.ReflectionTextureFilename:
                BitmapImage bitmapImage4 = this.LoadTexture(rootPath, str5);
                if (bitmapImage4 != null)
                {
                  currentMaterial.ReflectionTextureFilename = str5;
                  currentMaterial.ReflectionTexture = bitmapImage4;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.SpecularTextureFilename:
                BitmapImage bitmapImage5 = this.LoadTexture(rootPath, str5);
                if (bitmapImage5 != null)
                {
                  currentMaterial.SpecularTextureFilename = str5;
                  currentMaterial.SpecularTexture = bitmapImage5;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.ShininessTextureFilename:
                BitmapImage bitmapImage6 = this.LoadTexture(rootPath, str5);
                if (bitmapImage6 != null)
                {
                  currentMaterial.ShininessTextureFilename = str5;
                  currentMaterial.ShininessTexture = bitmapImage6;
                  continue;
                }
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.NewMaterial:
                if (currentMaterial.MaterialName != null && currentMaterial.Material == null)
                {
                  currentMaterial.Material = WavefrontObjAsset.WavefrontObjLoader.CreateMaterial(currentMaterial);
                  materialLibrary[currentMaterial.MaterialName] = currentMaterial;
                }
                currentMaterial = new WavefrontObjAsset.WavefrontObjLoader.ObjMaterial();
                currentMaterial.Alpha = 1.0;
                currentMaterial.MaterialName = str5;
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Shininess:
                double result3;
                if (double.TryParse(str5, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
                {
                  currentMaterial.Shininess = result3;
                  continue;
                }
                this.logger.WriteLine(StringTable.WavefrontObjLoaderErrorParsingShininess);
                continue;
              case WavefrontObjAsset.WavefrontObjLoader.KeywordType.Transparency:
                Color color5;
                WavefrontObjAsset.WavefrontObjLoader.ParseColor(str5, out color5);
                continue;
              default:
                continue;
            }
          }
        }
        if (currentMaterial.MaterialName == null || currentMaterial.Material != null)
          return;
        currentMaterial.Material = WavefrontObjAsset.WavefrontObjLoader.CreateMaterial(currentMaterial);
        materialLibrary[currentMaterial.MaterialName] = currentMaterial;
      }

      private BitmapImage LoadTexture(string rootPath, string content)
      {
        BitmapImage bitmapImage = (BitmapImage) null;
        if ((int) content[0] == 45)
        {
          content.IndexOfAny(WavefrontObjAsset.WavefrontObjLoader.Separators);
          content = content.Substring(content.LastIndexOfAny(WavefrontObjAsset.WavefrontObjLoader.Separators)).TrimStart(WavefrontObjAsset.WavefrontObjLoader.Separators);
        }
        try
        {
          string path = Path.Combine(rootPath, content);
          if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
          {
            IDocumentContext documentContext = this.sceneViewModel.XamlDocument.DocumentContext;
            DocumentReference reference = DocumentReference.Create(path);
            bitmapImage = new BitmapImage();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmapImage.UriSource = new Uri(DocumentReferenceLocator.GetDocumentReference(documentContext).GetRelativePath(reference), UriKind.RelativeOrAbsolute);
          }
        }
        catch (ArgumentException ex)
        {
        }
        return bitmapImage;
      }

      private static Material CreateMaterial(WavefrontObjAsset.WavefrontObjLoader.ObjMaterial currentMaterial)
      {
        MaterialGroup materialGroup = new MaterialGroup();
        if (currentMaterial.AmbientTexture != null)
        {
          materialGroup.Children.Add((Material) new EmissiveMaterial((Brush) new ImageBrush((ImageSource) currentMaterial.AmbientTexture)));
        }
        else
        {
          if (currentMaterial.Alpha < 1.0)
            currentMaterial.Ambient.ScA = (float) currentMaterial.Alpha;
          materialGroup.Children.Add((Material) new EmissiveMaterial((Brush) new SolidColorBrush(currentMaterial.Ambient)));
        }
        if (currentMaterial.DiffuseTexture != null)
        {
          materialGroup.Children.Add((Material) new DiffuseMaterial((Brush) new ImageBrush((ImageSource) currentMaterial.DiffuseTexture)));
        }
        else
        {
          if (currentMaterial.Alpha < 1.0)
            currentMaterial.Diffuse.ScA = (float) currentMaterial.Alpha;
          materialGroup.Children.Add((Material) new DiffuseMaterial((Brush) new SolidColorBrush(currentMaterial.Diffuse)));
        }
        if (currentMaterial.IlluminationMode > 1)
        {
          if (currentMaterial.SpecularTexture != null)
          {
            materialGroup.Children.Add((Material) new SpecularMaterial((Brush) new ImageBrush((ImageSource) currentMaterial.SpecularTexture), currentMaterial.Shininess));
          }
          else
          {
            if (currentMaterial.Alpha < 1.0)
              currentMaterial.Specular.ScA = (float) currentMaterial.Alpha;
            materialGroup.Children.Add((Material) new SpecularMaterial((Brush) new SolidColorBrush(currentMaterial.Specular), currentMaterial.Shininess));
          }
        }
        return (Material) materialGroup;
      }

      private static WavefrontObjAsset.WavefrontObjLoader.KeywordType ParseKeyword(string line)
      {
        if ((int) line[0] == 35 || (int) line[0] == 36)
          return WavefrontObjAsset.WavefrontObjLoader.KeywordType.Comment;
        foreach (WavefrontObjAsset.WavefrontObjLoader.KeywordData keywordData in WavefrontObjAsset.WavefrontObjLoader.Keywords)
        {
          if (string.Compare(line, keywordData.Name, StringComparison.OrdinalIgnoreCase) == 0)
            return keywordData.Type;
        }
        return WavefrontObjAsset.WavefrontObjLoader.KeywordType.UnrecognizedKeyword;
      }

      private static bool ParseFace(string content, out WavefrontObjAsset.WavefrontObjLoader.Face face)
      {
        face = new WavefrontObjAsset.WavefrontObjLoader.Face();
        face.CoordinateIndices = new List<int>();
        face.NormalIndices = new List<int>();
        face.TextureCoordinateIndices = new List<int>();
        foreach (string content1 in content.Split(WavefrontObjAsset.WavefrontObjLoader.Separators, StringSplitOptions.RemoveEmptyEntries))
        {
          int geometricVertexIndex;
          int textureVertexIndex;
          int vertexNormalIndex;
          if (!WavefrontObjAsset.WavefrontObjLoader.ParseVertexReference(content1, out geometricVertexIndex, out textureVertexIndex, out vertexNormalIndex))
            return false;
          face.CoordinateIndices.Add(geometricVertexIndex);
          face.NormalIndices.Add(vertexNormalIndex);
          face.TextureCoordinateIndices.Add(textureVertexIndex);
        }
        if (face.CoordinateIndices.Count < 3)
          return false;
        bool flag1 = face.TextureCoordinateIndices[0] != int.MaxValue;
        bool flag2 = face.NormalIndices[0] != int.MaxValue;
        for (int index = 0; index < face.CoordinateIndices.Count; ++index)
        {
          if (face.TextureCoordinateIndices[index] != int.MaxValue != flag1 || face.NormalIndices[index] != int.MaxValue != flag2)
            return false;
        }
        if (!flag1)
          face.TextureCoordinateIndices = (List<int>) null;
        if (!flag2)
          face.NormalIndices = (List<int>) null;
        return true;
      }

      private static bool ParseColor(string content, out Color color)
      {
        string[] strArray = content.Split(WavefrontObjAsset.WavefrontObjLoader.Separators, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length < 3)
        {
          color = new Color();
          return false;
        }
        float result1;
        float result2;
        float result3;
        bool flag = true & float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) & float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) & float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3);
        color = Color.FromScRgb(1f, result1, result2, result3);
        return flag;
      }

      private static bool ParseVertex(string content, out Point3D vertex)
      {
        string[] strArray = content.Split(WavefrontObjAsset.WavefrontObjLoader.Separators, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length < 3)
        {
          vertex = new Point3D();
          return false;
        }
        float result1;
        float result2;
        float result3;
        bool flag = true & float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) & float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) & float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3);
        vertex = new Point3D((double) result1, (double) result2, (double) result3);
        return flag;
      }

      private static bool ParseVertexNormal(string content, out Vector3D vector)
      {
        string[] strArray = content.Split(WavefrontObjAsset.WavefrontObjLoader.Separators, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length < 3)
        {
          vector = new Vector3D();
          return false;
        }
        float result1;
        float result2;
        float result3;
        bool flag = true & float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) & float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) & float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3);
        vector = new Vector3D((double) result1, (double) result2, (double) result3);
        return flag;
      }

      private static bool ParseVertexTextureCoordinate(string content, out Point textureCoordinate)
      {
        string[] strArray = content.Split(WavefrontObjAsset.WavefrontObjLoader.Separators, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length < 2)
        {
          textureCoordinate = new Point();
          return false;
        }
        float result1;
        float result2;
        bool flag = true & float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) & float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2);
        textureCoordinate = new Point((double) result1, 1.0 - (double) result2);
        return flag;
      }

      private static bool ParseVertexReference(string content, out int geometricVertexIndex, out int textureVertexIndex, out int vertexNormalIndex)
      {
        string[] strArray = content.Split('/');
        bool flag = false;
        geometricVertexIndex = int.MaxValue;
        textureVertexIndex = int.MaxValue;
        vertexNormalIndex = int.MaxValue;
        if (strArray.Length > 0)
          flag = int.TryParse(strArray[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out geometricVertexIndex);
        if (strArray.Length > 1 && strArray[1].Length > 0)
          flag &= int.TryParse(strArray[1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out textureVertexIndex);
        if (strArray.Length > 2 && strArray[2].Length > 0)
          flag &= int.TryParse(strArray[2], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out vertexNormalIndex);
        return flag;
      }

      private bool FinishCurrentGeometry(WavefrontObjAsset.WavefrontObjLoader.Group group, Material material, string name, string materialName)
      {
        if (name == null)
          name = materialName;
        if (this.faces.Count - 1 < this.nextFaceIndex)
          return false;
        group.Geometry.Add(new WavefrontObjAsset.WavefrontObjLoader.Geometry(name, material, this.nextFaceIndex, this.faces.Count - 1));
        this.nextFaceIndex = this.faces.Count;
        return true;
      }

      private void FinishCurrentGroup(WavefrontObjAsset.WavefrontObjLoader.Group group)
      {
        if (group.Geometry.Count <= 0)
          return;
        this.groups.Add(group);
      }

      private void SetName(DependencyObject target, string name)
      {
        ((ReferenceStep) this.sceneViewModel.ProjectContext.GetType(target.GetType()).Metadata.NameProperty).SetValue((object) target, (object) name);
      }

      private ModelVisual3D GenerateAvalonTree()
      {
        ModelVisual3D modelVisual3D1 = new ModelVisual3D();
        this.SetName((DependencyObject) modelVisual3D1, "RootGeometryContainer");
        for (int index1 = 0; index1 < this.groups.Count; ++index1)
        {
          WavefrontObjAsset.WavefrontObjLoader.Group group = this.groups[index1];
          ModelVisual3D modelVisual3D2 = new ModelVisual3D();
          this.SetName((DependencyObject) modelVisual3D2, SceneNodeIDHelper.ToCSharpID(group.Name));
          if (group.Geometry.Count > 1)
          {
            for (int index2 = 0; index2 < group.Geometry.Count; ++index2)
            {
              ModelVisual3D modelVisual3D3 = new ModelVisual3D();
              this.SetName((DependencyObject) modelVisual3D3, SceneNodeIDHelper.ToCSharpID(group.Geometry[index2].Name + "Container"));
              modelVisual3D3.Content = (Model3D) this.BuildGeometryModel3DFromGeometry(group.Geometry[index2]);
              if (modelVisual3D3.Content != null)
                modelVisual3D2.Children.Add((Visual3D) modelVisual3D3);
            }
          }
          else if (group.Geometry.Count == 1)
            modelVisual3D2.Content = (Model3D) this.BuildGeometryModel3DFromGeometry(group.Geometry[0]);
          modelVisual3D1.Children.Add((Visual3D) modelVisual3D2);
        }
        if (modelVisual3D1.Children.Count == 1)
          modelVisual3D1 = (ModelVisual3D) modelVisual3D1.Children[0];
        return modelVisual3D1;
      }

      private GeometryModel3D BuildGeometryModel3DFromGeometry(WavefrontObjAsset.WavefrontObjLoader.Geometry geometry)
      {
        GeometryModel3D geometryModel3D = new GeometryModel3D();
        MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
        SortedDictionary<WavefrontObjAsset.WavefrontObjLoader.UniqueVertex, int> sortedDictionary = new SortedDictionary<WavefrontObjAsset.WavefrontObjLoader.UniqueVertex, int>((IComparer<WavefrontObjAsset.WavefrontObjLoader.UniqueVertex>) new WavefrontObjAsset.WavefrontObjLoader.UniqueVertexComparer());
        meshGeometry3D.TriangleIndices = new Int32Collection();
        bool flag1 = false;
        bool flag2 = false;
        Rect3D empty = Rect3D.Empty;
        for (int firstFaceIndex = geometry.FirstFaceIndex; firstFaceIndex <= geometry.LastFaceIndex; ++firstFaceIndex)
        {
          WavefrontObjAsset.WavefrontObjLoader.Face face = this.faces[firstFaceIndex];
          int[] numArray = new int[face.CoordinateIndices.Count];
          for (int index = 0; index < face.CoordinateIndices.Count; ++index)
          {
            WavefrontObjAsset.WavefrontObjLoader.UniqueVertex key = new WavefrontObjAsset.WavefrontObjLoader.UniqueVertex();
            key.Coordinate = this.coordinates[face.CoordinateIndices[index]];
            if (face.NormalIndices != null)
            {
              key.Normal = this.vertexNormals[face.NormalIndices[index]];
              flag1 = true;
            }
            if (face.TextureCoordinateIndices != null)
            {
              key.TextureCoordinates = this.textureCoordinates[face.TextureCoordinateIndices[index]];
              flag2 = true;
            }
            int num = -1;
            if (!sortedDictionary.TryGetValue(key, out num))
            {
              num = sortedDictionary.Count;
              sortedDictionary.Add(key, num);
              empty.Union(key.Coordinate);
            }
            numArray[index] = num;
          }
          for (int index = 1; index < numArray.Length - 1; ++index)
          {
            meshGeometry3D.TriangleIndices.Add(numArray[0]);
            meshGeometry3D.TriangleIndices.Add(numArray[index]);
            meshGeometry3D.TriangleIndices.Add(numArray[index + 1]);
          }
        }
        int count = sortedDictionary.Count;
        if (count == 0)
          return (GeometryModel3D) null;
        if (this.generateTextureCoordinates)
          flag2 = true;
        meshGeometry3D.Positions = new Point3DCollection(count);
        if (flag1)
          meshGeometry3D.Normals = new Vector3DCollection(count);
        if (flag2)
          meshGeometry3D.TextureCoordinates = new PointCollection(count);
        Point3D center = empty.Location + new Vector3D(empty.SizeX / 2.0, empty.SizeY / 2.0, empty.SizeZ / 2.0);
        Point3D point3D = new Point3D();
        Vector3D vector3D = new Vector3D();
        Point point = new Point();
        for (int index = 0; index < count; ++index)
        {
          meshGeometry3D.Positions.Add(point3D);
          if (flag1)
            meshGeometry3D.Normals.Add(vector3D);
          if (flag2)
            meshGeometry3D.TextureCoordinates.Add(point);
        }
        foreach (KeyValuePair<WavefrontObjAsset.WavefrontObjLoader.UniqueVertex, int> keyValuePair in sortedDictionary)
        {
          meshGeometry3D.Positions[keyValuePair.Value] = keyValuePair.Key.Coordinate;
          if (keyValuePair.Key.NormalSet)
            meshGeometry3D.Normals[keyValuePair.Value] = keyValuePair.Key.Normal;
          meshGeometry3D.TextureCoordinates[keyValuePair.Value] = !keyValuePair.Key.TextureCoordinatesSet ? WavefrontObjAsset.WavefrontObjLoader.GetPositionalSphericalTextureCoordinateFromPoint(keyValuePair.Key.Coordinate, center) : keyValuePair.Key.TextureCoordinates;
        }
        geometryModel3D.Geometry = (Geometry3D) meshGeometry3D;
        geometryModel3D.Material = geometry.Material;
        this.SetName((DependencyObject) geometryModel3D, SceneNodeIDHelper.ToCSharpID(geometry.Name));
        return geometryModel3D;
      }

      private bool NormalizeFaceIndices(WavefrontObjAsset.WavefrontObjLoader.Face face)
      {
        for (int index1 = 0; index1 < face.CoordinateIndices.Count; ++index1)
        {
          if (face.CoordinateIndices[index1] < 0)
          {
            List<int> coordinateIndices;
            int index2;
            (coordinateIndices = face.CoordinateIndices)[index2 = index1] = coordinateIndices[index2] + this.coordinates.Count;
          }
          else
          {
            List<int> coordinateIndices;
            int index2;
            (coordinateIndices = face.CoordinateIndices)[index2 = index1] = coordinateIndices[index2] - 1;
          }
          if (face.CoordinateIndices[index1] >= this.coordinates.Count || face.CoordinateIndices[index1] < 0)
            return false;
          if (face.NormalIndices != null)
          {
            if (face.NormalIndices[index1] < 0)
            {
              List<int> normalIndices;
              int index2;
              (normalIndices = face.NormalIndices)[index2 = index1] = normalIndices[index2] + this.vertexNormals.Count;
            }
            else
            {
              List<int> normalIndices;
              int index2;
              (normalIndices = face.NormalIndices)[index2 = index1] = normalIndices[index2] - 1;
            }
            if (face.NormalIndices[index1] >= this.vertexNormals.Count || face.NormalIndices[index1] < -1)
              face.NormalIndices = (List<int>) null;
          }
          if (face.TextureCoordinateIndices != null)
          {
            if (face.TextureCoordinateIndices[index1] < 0)
            {
              List<int> coordinateIndices;
              int index2;
              (coordinateIndices = face.TextureCoordinateIndices)[index2 = index1] = coordinateIndices[index2] + this.textureCoordinates.Count;
            }
            else
            {
              List<int> coordinateIndices;
              int index2;
              (coordinateIndices = face.TextureCoordinateIndices)[index2 = index1] = coordinateIndices[index2] - 1;
            }
            if (face.TextureCoordinateIndices[index1] >= this.textureCoordinates.Count || face.TextureCoordinateIndices[index1] < -1)
              face.TextureCoordinateIndices = (List<int>) null;
          }
        }
        Vector3D vector1 = this.coordinates[face.CoordinateIndices[1]] - this.coordinates[face.CoordinateIndices[0]];
        Vector3D vector2 = this.coordinates[face.CoordinateIndices[2]] - this.coordinates[face.CoordinateIndices[0]];
        face.Normal = Vector3D.CrossProduct(vector1, vector2);
        return true;
      }

      private static Point GetPositionalSphericalTextureCoordinateFromPoint(Point3D point, Point3D center)
      {
        Vector3D vector3D = point - center;
        vector3D.Normalize();
        return new Point(Math.Asin(vector3D.X) / Math.PI + 0.5, 1.0 - (Math.Asin(vector3D.Y) / Math.PI + 0.5));
      }

      private enum KeywordType
      {
        InvalidKeyword,
        UnrecognizedKeyword,
        UnimplementedKeyword,
        Comment,
        Face,
        Group,
        MaterialLibrary,
        ObjectName,
        SmoothingGroup,
        UseMaterial,
        GeometricVertex,
        VertexNormal,
        TextureVertex,
        IlluminationMode,
        Alpha,
        Ambient,
        Diffuse,
        Specular,
        Emissive,
        AmbientTextureFilename,
        BumpTextureFilename,
        DiffuseTextureFilename,
        ReflectionTextureFilename,
        SpecularTextureFilename,
        ShininessTextureFilename,
        NewMaterial,
        RefractionIndex,
        Shininess,
        Sharpness,
        Transparency,
        Point,
        Line,
        Curve,
        Curve2D,
        Surface,
        MergingGroup,
        Call,
        UseMap,
      }

      private struct KeywordData
      {
        public string Name;
        public WavefrontObjAsset.WavefrontObjLoader.KeywordType Type;

        public KeywordData(string name, WavefrontObjAsset.WavefrontObjLoader.KeywordType type)
        {
          this.Name = name;
          this.Type = type;
        }
      }

      private class UniqueVertex
      {
        private Point3D coordinate;
        private Vector3D normal;
        private bool normalSet;
        private Point textureCoordinates;
        private bool textureCoordinatesSet;

        public Point3D Coordinate
        {
          get
          {
            return this.coordinate;
          }
          set
          {
            this.coordinate = value;
          }
        }

        public Vector3D Normal
        {
          get
          {
            return this.normal;
          }
          set
          {
            this.normal = value;
            this.normalSet = true;
          }
        }

        public bool NormalSet
        {
          get
          {
            return this.normalSet;
          }
        }

        public Point TextureCoordinates
        {
          get
          {
            return this.textureCoordinates;
          }
          set
          {
            this.textureCoordinates = value;
            this.textureCoordinatesSet = true;
          }
        }

        public bool TextureCoordinatesSet
        {
          get
          {
            return this.textureCoordinatesSet;
          }
        }

        public int CompareWithinTolerance(WavefrontObjAsset.WavefrontObjLoader.UniqueVertex other, double tolerance)
        {
          if (other.normalSet != this.normalSet)
            return other.normalSet.CompareTo(this.normalSet);
          if (other.textureCoordinatesSet != this.textureCoordinatesSet)
            return other.textureCoordinatesSet.CompareTo(this.textureCoordinatesSet);
          if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.coordinate.X, this.coordinate.X, tolerance))
            return other.coordinate.X.CompareTo(this.coordinate.X);
          if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.coordinate.Y, this.coordinate.Y, tolerance))
            return other.coordinate.Y.CompareTo(this.coordinate.Y);
          if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.coordinate.Z, this.coordinate.Z, tolerance))
            return other.coordinate.Z.CompareTo(this.coordinate.Z);
          if (this.normalSet)
          {
            if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.normal.X, this.normal.X, tolerance))
              return other.normal.X.CompareTo(this.normal.X);
            if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.normal.Y, this.normal.Y, tolerance))
              return other.normal.Y.CompareTo(this.normal.Y);
            if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.normal.Z, this.normal.Z, tolerance))
              return other.normal.Z.CompareTo(this.normal.X);
          }
          if (this.textureCoordinatesSet)
          {
            if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.textureCoordinates.X, this.textureCoordinates.X, tolerance))
              return other.textureCoordinates.X.CompareTo(this.textureCoordinates.X);
            if (!WavefrontObjAsset.WavefrontObjLoader.UniqueVertex.DoubleIsWithinToleranceOf(other.textureCoordinates.Y, this.textureCoordinates.Y, tolerance))
              return other.textureCoordinates.Y.CompareTo(this.textureCoordinates.Y);
          }
          return 0;
        }

        private static bool DoubleIsWithinToleranceOf(double lhs, double rhs, double tolerance)
        {
          return Math.Abs(lhs - rhs) < tolerance;
        }
      }

      private class UniqueVertexComparer : Comparer<WavefrontObjAsset.WavefrontObjLoader.UniqueVertex>
      {
        public override int Compare(WavefrontObjAsset.WavefrontObjLoader.UniqueVertex x, WavefrontObjAsset.WavefrontObjLoader.UniqueVertex y)
        {
          return x.CompareWithinTolerance(y, WavefrontObjAsset.WavefrontObjLoader.GeneratedVertexTolerance);
        }
      }

      private class Geometry
      {
        public string Name { get; private set; }

        public Material Material { get; private set; }

        public int FirstFaceIndex { get; private set; }

        public int LastFaceIndex { get; private set; }

        public Geometry(string name, Material material, int firstFaceIndex, int lastFaceIndex)
        {
          this.Name = name;
          this.Material = material;
          this.FirstFaceIndex = firstFaceIndex;
          this.LastFaceIndex = lastFaceIndex;
        }
      }

      private class Group
      {
        private List<WavefrontObjAsset.WavefrontObjLoader.Geometry> geometry = new List<WavefrontObjAsset.WavefrontObjLoader.Geometry>();

        public string Name { get; private set; }

        public List<WavefrontObjAsset.WavefrontObjLoader.Geometry> Geometry
        {
          get
          {
            return this.geometry;
          }
        }

        public Group(string name)
        {
          this.Name = name;
        }
      }

      private class Face
      {
        private List<int> coordinateIndices;
        private List<int> normalIndices;
        private List<int> textureCoordinateIndices;
        private Vector3D normal;

        public List<int> CoordinateIndices
        {
          get
          {
            return this.coordinateIndices;
          }
          set
          {
            this.coordinateIndices = value;
          }
        }

        public List<int> NormalIndices
        {
          get
          {
            return this.normalIndices;
          }
          set
          {
            this.normalIndices = value;
          }
        }

        public List<int> TextureCoordinateIndices
        {
          get
          {
            return this.textureCoordinateIndices;
          }
          set
          {
            this.textureCoordinateIndices = value;
          }
        }

        public Vector3D Normal
        {
          get
          {
            return this.normal;
          }
          set
          {
            this.normal = value;
          }
        }
      }

      private struct ObjMaterial
      {
        public int IlluminationMode;
        public Color Ambient;
        public Color Diffuse;
        public Color Specular;
        public Color Emissive;
        public double Alpha;
        public double Shininess;
        public string MaterialName;
        public string AmbientTextureFilename;
        public string BumpTextureFilename;
        public string DiffuseTextureFilename;
        public string ReflectionTextureFilename;
        public string SpecularTextureFilename;
        public string ShininessTextureFilename;
        public BitmapImage AmbientTexture;
        public BitmapImage BumpTexture;
        public BitmapImage DiffuseTexture;
        public BitmapImage ReflectionTexture;
        public BitmapImage SpecularTexture;
        public BitmapImage ShininessTexture;
        public Material Material;
      }
    }
  }
}
