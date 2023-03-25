namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Production;

    internal sealed class DefaultModifier : IModifier<NetCoreProjectFile>
    {
        private NetCoreProjectFile _file;

        internal DefaultModifier(NetCoreProjectFile file)
        {
            this._file = file;
        }

        public SyncResult RemoveElements(List<XElement> elements)
        {
            SyncResult result = SyncResult.Succeed;
            foreach (var element in elements)
            {
                if (this.TryFindElement(element, out XElement target) && target.TryRemove())
                {
                    ;
                }
                else
                {
                    result = SyncResult.Failed;
                }
            }
            return result;
        }

        public SyncResult AddElementsAfter(XElement element, List<XElement> elements)
        {
            if (this.TryFindElement(element, out XElement target))
            {
                target.AddAfterSelf(elements);
                return SyncResult.Succeed;
            }
            else
            {
                return this.AddElements(elements);
            }
        }

        public SyncResult AddElements(List<XElement> elements)
        {
            string tag = elements.First().Name.LocalName;

            if (this.TryAddElementsByTag(tag, elements))
            {
                return SyncResult.Succeed;
            }
            else
            {
                // is it a property?
                if (NetFrameworkConsts.KnownProperties.Contains(tag))
                {
                    if (this.TryAddAsProperties(elements))
                    {
                        return SyncResult.Succeed;
                    }
                    else
                    {
                        return SyncResult.Failed;
                    }
                }

                // otherwise add it to a new itemgroup
                else
                {
                    if (this.TryAddAsNewItemGroup(elements))
                    {
                        return SyncResult.Succeed;
                    }
                    else if (this.TryAddToButtom(elements))
                    {
                        return SyncResult.Partially;
                    }
                    else
                    {
                        return SyncResult.Failed;
                    }
                }
            }
        }

        // ------------------------------------------------------------

        private bool TryFindElement(XElement target, out XElement result)
        {
            result = null;

            var sameTagElements = this._file.Document.GetAll(target.Name.LocalName);

            if (target.HasAttribute(Tags.Include))
            {
                string tInclude = target.GetAttribute(Tags.Include).Value.Split("\\").Last();
                foreach (var element in sameTagElements)
                {
                    string eInclude = element.GetAttribute(Tags.Include)?.Value.Split("\\").Last();
                    if (StringUtils.EqualsIgnoreCase(tInclude, eInclude))
                    {
                        result = element;
                        break;
                    }
                }
            }

            else if (target.HasAttribute(Tags.Name))
            {
                string tName = target.GetAttribute(Tags.Name).Value;
                foreach (var element in sameTagElements)
                {
                    string eName = element.GetAttribute(Tags.Name)?.Value;
                    if (StringUtils.EqualsIgnoreCase(tName, eName))
                    {
                        result = element;
                        break;
                    }
                }
            }

            else
            {
                string tString = target.ToString();
                foreach (var element in sameTagElements)
                {
                    string eString = element.ToString();
                    if (StringUtils.EqualsIgnoreCase(tString, eString))
                    {
                        result = element;
                        break;
                    }
                }
            }

            return result != null;
        }

        private bool TryAddElementsByTag(string tag, List<XElement> elements)
        {
            var first = this._file.Document.GetAll(tag).FirstOrDefault();

            if (first != null)
            {
                first.AddBeforeSelf(elements);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryAddAsProperties(List<XElement> elements)
        {
            var addList = elements.Where(e => !NetFrameworkConsts.NetFrameworkOnylProperties.Contains(e.Name.LocalName))
                                  .ToList();

            if (!addList.Any())
            {
                return true;
            }

            var mainGroup = this._file.Document.GetAll(Tags.PropertyGroup)
                                               .Where(g => g.GetFirst(Tags.AssemblyName) != null)
                                               .FirstOrDefault();
            if (mainGroup == null)
            {
                return false;
            }

            mainGroup.Add(addList);
            return true;
        }

        private bool TryAddAsNewItemGroup(List<XElement> elements)
        {
            try
            {
                var itemGroup = new XElement(Tags.ItemGroup);
                itemGroup.Add(elements);

                var lastGroup = this._file.Document.GetAll(Tags.ItemGroup).LastOrDefault() ??
                                  this._file.Document.GetAll(Tags.PropertyGroup).LastOrDefault();
                if (lastGroup != null)
                {
                    lastGroup.AddAfterSelf(itemGroup);
                }
                else
                {
                    this._file.Document.Root.Add(itemGroup);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryAddToButtom(List<XElement> elements)
        {
            try
            {
                this._file.Document.Root.Add(elements);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
