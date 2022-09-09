namespace SyncTool
{
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Porting;

    internal class SyncJob
    {
        internal string SourceFilePath { get; set; }
        internal string ProducedPath { get; set; }

        internal BuildFile? MainFile { get; set; }
        internal BuildFile? DFFile { get; set; }
        internal BuildFile ProducedFile { get; set; }
        internal string Framework { get; set; }

        public SyncJob(string sourceFilePath, string producedPath, BuildFile producedFile, string framework)
        {
            this.SourceFilePath = sourceFilePath;
            this.ProducedPath = producedPath;
            this.ProducedFile = producedFile;
            this.Framework = framework;
        }

        internal SyncResult Execute(LookupTable lookupTable)
        {
            var metadata = new PortingMetadata(this.SourceFilePath, this.Framework);
            var resolver = new ReferenceResolver(metadata.NFBuildFile, lookupTable.KnownSDKs);
            var converter = new ElementConverter(metadata, lookupTable, resolver);
            var formater = new BuildFileFormater(lookupTable);
            var comparer = new BuildFileComparer(converter);

            if (MainFile == null || DFFile == null)
            {
                return SyncResult.NotChanged;
            }

            var diffs = comparer.Compare(MainFile, DFFile);

            var overAllResult = SyncResult.Succeed;

            foreach (var diff in diffs)
            {
                var result = diff.SyncTo(ProducedFile);
            }

            formater.Format(ProducedFile);

            ProducedFile.Save(omniDeclaration: true);

            //TODO: overall result
            return overAllResult;
        }
    }
}
