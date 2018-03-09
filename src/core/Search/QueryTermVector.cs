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

using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Index;
using Lucene.Net.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer = Lucene.Net.Analysis.Analyzer;
using TokenStream = Lucene.Net.Analysis.TokenStream;

namespace Lucene.Net.Search
{
    /// <summary>
    ///
    ///
    /// </summary>
    public class QueryTermVector : ITermFreqVector
    {
        private string[] terms = new string[0];
        private int[] termFreqs = new int[0];

        public virtual string Field
        {
            get { return null; }
        }

        /// <summary> </summary>
        /// <param name="queryTerms">The original list of terms from the query, can contain duplicates
        /// </param>
        public QueryTermVector(string[] queryTerms)
        {
            ProcessTerms(queryTerms);
        }

        public QueryTermVector(string queryString, Analyzer analyzer)
        {
            if (analyzer != null)
            {
                TokenStream stream = analyzer.TokenStream("", new StringReader(queryString));
                if (stream != null)
                {
                    IList<string> terms = new List<string>();
                    try
                    {
                        bool hasMoreTokens = false;

                        stream.Reset();
                        ITermAttribute termAtt = stream.AddAttribute<ITermAttribute>();

                        hasMoreTokens = stream.IncrementToken();
                        while (hasMoreTokens)
                        {
                            terms.Add(termAtt.Term);
                            hasMoreTokens = stream.IncrementToken();
                        }
                        ProcessTerms(terms.ToArray());
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        private void ProcessTerms(string[] queryTerms)
        {
            if (queryTerms != null)
            {
                Array.Sort(queryTerms);
                IDictionary<string, int> tmpSet = new HashMap<string, int>(queryTerms.Length);
                //filter out duplicates
                IList<string> tmpList = new List<string>(queryTerms.Length);
                IList<int> tmpFreqs = new List<int>(queryTerms.Length);
                int j = 0;
                for (int i = 0; i < queryTerms.Length; i++)
                {
                    var term = queryTerms[i];
                    var position = tmpSet[term];
                    if (!tmpSet.ContainsKey(term)) // if temp_position == null
                    {
                        tmpSet[term] = j++;
                        tmpList.Add(term);
                        tmpFreqs.Add(1);
                    }
                    else
                    {
                        int integer = tmpFreqs[position];
                        tmpFreqs[position] = (integer + 1);
                    }
                }
                terms = tmpList.ToArray();
                //termFreqs = (int[])tmpFreqs.toArray(termFreqs);
                termFreqs = new int[tmpFreqs.Count];
                int i2 = 0;
                foreach (int integer in tmpFreqs)
                {
                    termFreqs[i2++] = integer;
                }
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append('{');
            for (int i = 0; i < terms.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(terms[i]).Append('/').Append(termFreqs[i]);
            }
            sb.Append('}');
            return sb.ToString();
        }

        public virtual int Size
        {
            get { return terms.Length; }
        }

        public virtual string[] GetTerms()
        {
            return terms;
        }

        public virtual int[] GetTermFrequencies()
        {
            return termFreqs;
        }

        public virtual int IndexOf(string term)
        {
            int res = Array.BinarySearch(terms, term);
            return res >= 0 ? res : -1;
        }

        public virtual int[] IndexesOf(string[] terms, int start, int len)
        {
            int[] res = new int[len];

            for (int i = 0; i < len; i++)
            {
                res[i] = IndexOf(terms[i]);
            }
            return res;
        }
    }
}