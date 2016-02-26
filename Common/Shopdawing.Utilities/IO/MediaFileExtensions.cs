// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.MediaFileExtensions
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Utility.IO
{
  public static class MediaFileExtensions
  {
    private static readonly string[] silverlightAndWpfMediaExtensions = new string[6]
    {
      ".mp3",
      ".mp4",
      ".asf",
      ".asx",
      ".wma",
      ".wmv"
    };
    private static readonly string[] wpfMediaExtensions = new string[21]
    {
      ".dvr-ms",
      ".mid",
      ".rmi",
      ".midi",
      ".mpeg",
      ".mpg",
      ".m1v",
      ".mp2",
      ".mpa",
      ".mpe",
      ".ifo",
      ".vob",
      ".wav",
      ".snd",
      ".au",
      ".aif",
      ".aifc",
      ".aiff",
      ".wm",
      ".wmd",
      ".avi"
    };
    private static readonly string[] webAudioExtensions = new string[4]
    {
      ".mp3",
      ".m4a",
      ".wav",
      ".wma"
    };
    private static readonly string[] webVideoExtensions = new string[8]
    {
      ".avi",
      ".mpeg",
      ".mp4",
      ".m4v",
      ".mov",
      ".webm",
      ".asf",
      ".wmv"
    };
    private static readonly string[] waveFrontObjExtensions = new string[1]
    {
      ".obj"
    };
    private static readonly IDictionary<string, string[]> imageExtensionLookup = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "image/gif",
        new string[1]
        {
          ".gif"
        }
      },
      {
        "image/ico",
        new string[1]
        {
          ".ico"
        }
      },
      {
        "image/jpg",
        new string[2]
        {
          ".jpg",
          ".jpeg"
        }
      },
      {
        "image/png",
        new string[1]
        {
          ".png"
        }
      },
      {
        "image/tif",
        new string[2]
        {
          ".tif",
          ".tiff"
        }
      },
      {
        "image/bmp",
        new string[1]
        {
          ".bmp"
        }
      }
    };

    public static string[] SilverlightAndWpfMediaExtensions
    {
      get
      {
        return MediaFileExtensions.silverlightAndWpfMediaExtensions;
      }
    }

    public static string[] WpfMediaExtensions
    {
      get
      {
        return MediaFileExtensions.wpfMediaExtensions;
      }
    }

    public static string[] WebAudioExtensions
    {
      get
      {
        return MediaFileExtensions.webAudioExtensions;
      }
    }

    public static string[] WebVideoExtensions
    {
      get
      {
        return MediaFileExtensions.webVideoExtensions;
      }
    }

    public static string[] WaveFrontObjExtensions
    {
      get
      {
        return MediaFileExtensions.waveFrontObjExtensions;
      }
    }

    public static IDictionary<string, string[]> ImageExtensionLookup
    {
      get
      {
        return MediaFileExtensions.imageExtensionLookup;
      }
    }

    public static IEnumerable<string> AllImageExtensions
    {
      get
      {
        foreach (string[] strArray in (IEnumerable<string[]>) MediaFileExtensions.ImageExtensionLookup.Values)
        {
          foreach (string str in strArray)
            yield return str;
        }
      }
    }
  }
}
