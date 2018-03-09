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

namespace Lucene.Net.Index
{
    internal abstract class TermsHashConsumer
    {
        internal abstract int BytesPerPosting();

        internal abstract void CreatePostings(RawPostingList[] postings, int start, int count);

        public abstract TermsHashConsumerPerThread AddThread(TermsHashPerThread perThread);

        public abstract void Flush(IDictionary<TermsHashConsumerPerThread, ICollection<TermsHashConsumerPerField>> threadsAndFields, SegmentWriteState state);

        public abstract void Abort();

        internal abstract void CloseDocStore(SegmentWriteState state);

        internal FieldInfos fieldInfos;

        internal virtual void SetFieldInfos(FieldInfos fieldInfos)
        {
            this.fieldInfos = fieldInfos;
        }
    }
}