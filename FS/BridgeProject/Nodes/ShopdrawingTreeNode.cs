using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.TreeView;

namespace Shopdrawing.TreeNode
{
    /// <summary>
    /// Base class of all ILSpy tree nodes.
    /// </summary>
    public abstract class ShopdrawingTreeNode : SharpTreeNode
    {
        public virtual object DataObject { get; set; }

        FilterSettings filterSettings;
        bool childrenNeedFiltering;

        public FilterSettings FilterSettings
        {
            get { return filterSettings; }
            set
            {
                if (filterSettings != value)
                {
                    filterSettings = value;
                    OnFilterSettingsChanged();
                }
            }
        }


        public virtual FilterResult Filter(FilterSettings settings)
        {
            if (string.IsNullOrEmpty(settings.SearchTerm))
                return FilterResult.Match;
            else
                return FilterResult.Hidden;
        }

        protected static object HighlightSearchMatch(string text, string suffix = null)
        {
            // TODO: implement highlighting the search match
            return text + suffix;
        }


        protected override void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (IsVisible)
                {
                    foreach (ShopdrawingTreeNode node in e.NewItems)
                        ApplyFilterToChild(node);
                }
                else
                {
                    childrenNeedFiltering = true;
                }
            }
            base.OnChildrenChanged(e);
        }

        void ApplyFilterToChild(ShopdrawingTreeNode child)
        {
            FilterResult r;
            if (this.FilterSettings == null)
                r = FilterResult.Match;
            else
                r = child.Filter(this.FilterSettings);
            switch (r)
            {
                case FilterResult.Hidden:
                    child.IsHidden = true;
                    break;
                case FilterResult.Match:
                    child.FilterSettings = StripSearchTerm(this.FilterSettings);
                    child.IsHidden = false;
                    break;
                case FilterResult.Recurse:
                    child.FilterSettings = this.FilterSettings;
                    child.EnsureChildrenFiltered();
                    child.IsHidden = child.Children.All(c => c.IsHidden);
                    break;
                case FilterResult.MatchAndRecurse:
                    child.FilterSettings = StripSearchTerm(this.FilterSettings);
                    child.EnsureChildrenFiltered();
                    child.IsHidden = child.Children.All(c => c.IsHidden);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        static FilterSettings StripSearchTerm(FilterSettings filterSettings)
        {
            if (filterSettings == null)
                return null;
            if (!string.IsNullOrEmpty(filterSettings.SearchTerm))
            {
                filterSettings = filterSettings.Clone();
                filterSettings.SearchTerm = null;
            }
            return filterSettings;
        }

        protected virtual void OnFilterSettingsChanged()
        {
            RaisePropertyChanged("Text");
            if (IsVisible)
            {
                foreach (ShopdrawingTreeNode node in this.Children.OfType<ShopdrawingTreeNode>())
                    ApplyFilterToChild(node);
            }
            else
            {
                childrenNeedFiltering = true;
            }
        }

        protected override void OnIsVisibleChanged()
        {
            base.OnIsVisibleChanged();
            EnsureChildrenFiltered();
        }

        void EnsureChildrenFiltered()
        {
            EnsureLazyChildren();
            if (childrenNeedFiltering)
            {
                childrenNeedFiltering = false;
                foreach (ShopdrawingTreeNode node in this.Children.OfType<ShopdrawingTreeNode>())
                    ApplyFilterToChild(node);
            }
        }

        public virtual bool IsPublicAPI
        {
            get { return true; }
        }

        public virtual bool IsAutoLoaded
        {
            get { return false; }
        }

        public override System.Windows.Media.Brush Foreground
        {
            get
            {
                if (IsPublicAPI)
                    if (IsAutoLoaded)
                    {
                        // HACK: should not be hard coded?
                        return System.Windows.Media.Brushes.SteelBlue;
                    }
                    else
                    {
                        return base.Foreground;
                    }
                else
                    return System.Windows.SystemColors.GrayTextBrush;
            }
        }
    }
}