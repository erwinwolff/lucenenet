/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using Directory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Index
{
    internal class SegmentWriteState
    {
        internal DocumentsWriter docWriter;
        internal Directory directory;
        internal string segmentName;
        internal string docStoreSegmentName;
        internal int numDocs;
        internal int termIndexInterval;
        internal int numDocsInStore;
        internal ICollection<string> flushedFiles;

        public SegmentWriteState(DocumentsWriter docWriter, Directory directory, string segmentName, string docStoreSegmentName, int numDocs, int numDocsInStore, int termIndexInterval)
        {
            this.docWriter = docWriter;
            this.directory = directory;
            this.segmentName = segmentName;
            this.docStoreSegmentName = docStoreSegmentName;
            this.numDocs = numDocs;
            this.numDocsInStore = numDocsInStore;
            this.termIndexInterval = termIndexInterval;
            flushedFiles = new HashSet<string>();
        }

        public virtual string SegmentFileName(string ext)
        {
            return segmentName + "." + ext;
        }
    }
}