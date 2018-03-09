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

namespace Lucene.Net.Store
{
    /// <summary> Expert: A Directory instance that switches files between
    /// two other Directory instances.
    /// <p/>Files with the specified extensions are placed in the
    /// primary directory; others are placed in the secondary
    /// directory.  The provided Set must not change once passed
    /// to this class, and must allow multiple threads to call
    /// contains at once.<p/>
    ///
    /// <p/><b>NOTE</b>: this API is new and experimental and is
    /// subject to suddenly change in the next release.
    /// </summary>

    public class FileSwitchDirectory : Directory
    {
        private Directory secondaryDir;
        private Directory primaryDir;
        private HashSet<string> primaryExtensions;
        private bool doClose;
        private bool isDisposed;

        public FileSwitchDirectory(HashSet<string> primaryExtensions,
                                    Directory primaryDir,
                                    Directory secondaryDir,
                                    bool doClose)
        {
            this.primaryExtensions = primaryExtensions;
            this.primaryDir = primaryDir;
            this.secondaryDir = secondaryDir;
            this.doClose = doClose;
            this.interalLockFactory = primaryDir.LockFactory;
        }

        /// <summary>Return the primary directory </summary>
        public virtual Directory PrimaryDir
        {
            get { return primaryDir; }
        }

        /// <summary>Return the secondary directory </summary>
        public virtual Directory SecondaryDir
        {
            get { return secondaryDir; }
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (doClose)
            {
                try
                {
                    if (secondaryDir != null)
                    {
                        secondaryDir.Close();
                    }
                }
                finally
                {
                    if (primaryDir != null)
                    {
                        primaryDir.Close();
                    }
                }
                doClose = false;
            }

            secondaryDir = null;
            primaryDir = null;
            isDisposed = true;
        }

        public override string[] ListAll()
        {
            var files = new List<string>();
            files.AddRange(primaryDir.ListAll());
            files.AddRange(secondaryDir.ListAll());
            return files.ToArray();
        }

        /// <summary>Utility method to return a file's extension. </summary>
        public static string GetExtension(string name)
        {
            int i = name.LastIndexOf('.');
            if (i == -1)
            {
                return "";
            }
            return name.Substring(i + 1, (name.Length) - (i + 1));
        }

        private Directory GetDirectory(string name)
        {
            string ext = GetExtension(name);
            if (primaryExtensions.Contains(ext))
            {
                return primaryDir;
            }
            else
            {
                return secondaryDir;
            }
        }

        public override bool FileExists(string name)
        {
            return GetDirectory(name).FileExists(name);
        }

        public override long FileModified(string name)
        {
            return GetDirectory(name).FileModified(name);
        }

        public override void TouchFile(string name)
        {
            GetDirectory(name).TouchFile(name);
        }

        public override void DeleteFile(string name)
        {
            GetDirectory(name).DeleteFile(name);
        }

        public override long FileLength(string name)
        {
            return GetDirectory(name).FileLength(name);
        }

        public override IndexOutput CreateOutput(string name)
        {
            return GetDirectory(name).CreateOutput(name);
        }

        public override void Sync(string name)
        {
            GetDirectory(name).Sync(name);
        }

        public override IndexInput OpenInput(string name)
        {
            return GetDirectory(name).OpenInput(name);
        }
    }
}