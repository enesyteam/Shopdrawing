// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.TreeStatistics
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class TreeStatistics
  {
    private int visualCount;
    private int greatestVisualDepth;
    private int depthTotal;
    private StringBuilder trackingBuffer;
    private FrameworkElement root;
    private Dictionary<Type, int> elementCounts;

    public Dictionary<Type, int> ElementCounts
    {
      get
      {
        return this.elementCounts;
      }
    }

    public int VisualCount
    {
      get
      {
        return this.visualCount;
      }
    }

    public int GreatestVisualDepth
    {
      get
      {
        return this.greatestVisualDepth;
      }
    }

    public double AverageVisualDepth
    {
      get
      {
        return (double) this.depthTotal / (double) this.visualCount;
      }
    }

    public TreeStatistics(FrameworkElement root)
    {
      this.root = root;
    }

    public void GetStatistics()
    {
      this.visualCount = 0;
      this.greatestVisualDepth = 0;
      this.depthTotal = 0;
      this.elementCounts = new Dictionary<Type, int>();
      VisualTreeWalker visualTreeWalker = new VisualTreeWalker((Visual) this.root);
      visualTreeWalker.VisualVisited += new VisualVisitor(this.StatisticsVisitor);
      visualTreeWalker.Walk();
      visualTreeWalker.VisualVisited -= new VisualVisitor(this.StatisticsVisitor);
    }

    private void StatisticsVisitor(Visual v, int currentDepth)
    {
      this.depthTotal += currentDepth;
      ++this.visualCount;
      if (currentDepth > this.greatestVisualDepth)
        this.greatestVisualDepth = currentDepth;
      this.CountElement(v.GetType());
    }

    private void CountElement(Type t)
    {
      int num1;
      if (this.elementCounts.TryGetValue(t, out num1))
      {
        int num2 = num1 + 1;
        this.elementCounts[t] = num2;
      }
      else
        this.elementCounts.Add(t, 1);
    }

    public void WriteTreeToFile(string filename)
    {
      this.trackingBuffer = new StringBuilder(50000);
      VisualTreeWalker visualTreeWalker = new VisualTreeWalker((Visual) this.root);
      visualTreeWalker.VisualVisited += new VisualVisitor(this.TreeDumpVisitor);
      visualTreeWalker.Walk();
      visualTreeWalker.VisualVisited -= new VisualVisitor(this.TreeDumpVisitor);
      using (StreamWriter text = File.CreateText(filename))
        text.WriteLine(this.trackingBuffer.ToString());
    }

    private void TreeDumpVisitor(Visual visual, int currentDepth)
    {
      this.AppendIndentString(currentDepth);
      this.trackingBuffer.Append("Type = '");
      this.trackingBuffer.Append(visual.GetType().Name);
      FrameworkElement frameworkElement = visual as FrameworkElement;
      if (frameworkElement != null)
      {
        this.trackingBuffer.Append("', Name = '");
        this.trackingBuffer.Append(frameworkElement.Name);
        this.trackingBuffer.Append("', ActualWidth = '");
        this.trackingBuffer.Append(frameworkElement.ActualWidth);
        this.trackingBuffer.Append("', MinHeight = '");
        this.trackingBuffer.Append(frameworkElement.MinHeight);
        this.trackingBuffer.Append("', Height = '");
        this.trackingBuffer.Append(frameworkElement.Height);
        this.trackingBuffer.Append("', ActualHeight = '");
        this.trackingBuffer.Append(frameworkElement.ActualHeight);
      }
      this.trackingBuffer.AppendLine("', Hash = " + visual.GetHashCode().ToString());
    }

    private void AppendIndentString(int currentDepth)
    {
      for (int index = 1; index < currentDepth; ++index)
      {
        if (index % 10 == 0)
          this.trackingBuffer.Append("|");
        else if (index % 5 == 0)
          this.trackingBuffer.Append("+");
        else
          this.trackingBuffer.Append("-");
      }
    }
  }
}
