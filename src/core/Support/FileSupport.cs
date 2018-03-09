/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

using System;
using System.Collections;
using System.IO;

namespace Lucene.Net.Support
{
    /// <summary>
    /// Represents the methods to support some operations over files.
    /// </summary>
    public class FileSupport
    {
        /// <summary>
        /// Returns an array of abstract pathnames representing the files and directories of the specified path.
        /// </summary>
        /// <param name="path">The abstract pathname to list it childs.</param>
        /// <returns>An array of abstract pathnames childs of the path specified or null if the path is not a directory</returns>
        public static FileInfo[] GetFiles(FileInfo path)
        {
            if ((path.Attributes & FileAttributes.Directory) > 0)
            {
                String[] fullpathnames = Directory.GetFileSystemEntries(path.FullName);
                FileInfo[] result = new FileInfo[fullpathnames.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = new FileInfo(fullpathnames[i]);
                return result;
            }
            else
                return null;
        }

        // TODO: This filesupport thing is silly.  Same goes with _TestUtil's RMDir.
        //       If we're removing a directory
        public static FileInfo[] GetFiles(DirectoryInfo path)
        {
            return GetFiles(new FileInfo(path.FullName));
        }

        /// <summary>
        /// Returns a list of files in a give directory.
        /// </summary>
        /// <param name="fullName">The full path name to the directory.</param>
        /// <param name="indexFileNameFilter"></param>
        /// <returns>An array containing the files.</returns>
        public static string[] GetLuceneIndexFiles(string fullName,
                                                          Index.IndexFileNameFilter indexFileNameFilter)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullName);
            ArrayList list = new ArrayList();
            foreach (FileInfo fInfo in dInfo.GetFiles())
            {
                if (indexFileNameFilter.Accept(fInfo, fInfo.Name) == true)
                {
                    list.Add(fInfo.Name);
                }
            }
            string[] retFiles = new string[list.Count];
            list.CopyTo(retFiles);
            return retFiles;
        }

        // Disable the obsolete warning since we must use FileStream.Handle
        // because Mono does not support FileSystem.SafeFileHandle at present.
#pragma warning disable 618

        /// <summary>
        /// Flushes the specified file stream. Ensures that all buffered
        /// data is actually written to the file system.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        public static void Sync(FileStream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            fileStream.Flush();

            //if (OS.IsWindows)
            //{
            //    if (!FlushFileBuffers(fileStream.Handle))
            //        throw new IOException();
            //}
            //else if (OS.IsUnix)
            //{
            //    if (fsync(fileStream.Handle) != IntPtr.Zero)
            //    throw new IOException();
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
        }

#pragma warning restore 618

        //[System.Runtime.InteropServices.DllImport("libc")]
        //extern static IntPtr fsync(IntPtr fd);

        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //extern static bool FlushFileBuffers(IntPtr hFile);
    }
}