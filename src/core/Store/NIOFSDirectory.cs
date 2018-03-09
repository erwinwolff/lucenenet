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

using System.IO;

namespace Lucene.Net.Store
{
    /// <summary>
    /// Not implemented. Waiting for volunteers.
    /// </summary>
    public class NIOFSDirectory : Lucene.Net.Store.FSDirectory
    {
        public NIOFSDirectory(DirectoryInfo dir, LockFactory lockFactory)
            : base(dir, lockFactory)
        {
            throw new System.NotImplementedException("Waiting for volunteers to implement this class");
        }

        /// <summary>
        /// Not implemented. Waiting for volunteers.
        /// </summary>
        public class NIOFSIndexInput
        {
            public NIOFSIndexInput()
            {
                throw new System.NotImplementedException("Waiting for volunteers to implement this class");
            }
        }

        public override IndexOutput CreateOutput(string name)
        {
            throw new System.NotImplementedException("Waiting for volunteers to implement this class");
        }
    }
}

//namespace Lucene.Net.Store
//{
//    /// <summary> An <see cref="FSDirectory" /> implementation that uses
//    /// java.nio's FileChannel's positional read, which allows
//    /// multiple threads to read from the same file without
//    /// synchronizing.
//    ///
//    /// <p/>This class only uses FileChannel when reading; writing
//    /// is achieved with <see cref="SimpleFSDirectory.SimpleFSIndexOutput" />.
//    ///
//    /// <p/><b>NOTE</b>: NIOFSDirectory is not recommended on Windows because of a bug
//    /// in how FileChannel.read is implemented in Sun's JRE.
//    /// Inside of the implementation the position is apparently
//    /// synchronized.  See <a
//    /// href="http://bugs.sun.com/bugdatabase/view_bug.do?bug_id=6265734">here</a>
//    /// for details.
//    /// </summary>
//    public class NIOFSDirectory:FSDirectory
//    {
//        /// <summary>Create a new NIOFSDirectory for the named location.
//        ///
//        /// </summary>
//        /// <param name="path">the path of the directory
//        /// </param>
//        /// <param name="lockFactory">the lock factory to use, or null for the default.
//        /// </param>
//        /// <throws>  IOException </throws>
//        [System.Obsolete("Use the constructor that takes a DirectoryInfo, this will be removed in the 3.0 release")]
//        public NIOFSDirectory(FileInfo path, LockFactory lockFactory):base(new DirectoryInfo(path.FullName), lockFactory)
//        {
//        }

//        /// <summary>Create a new NIOFSDirectory for the named location.
//        ///
//        /// </summary>
//        /// <param name="path">the path of the directory
//        /// </param>
//        /// <param name="lockFactory">the lock factory to use, or null for the default.
//        /// </param>
//        /// <throws>  IOException </throws>
//        public NIOFSDirectory(DirectoryInfo path, LockFactory lockFactory) : base(path, lockFactory)
//        {
//        }

//        /// <summary>Create a new NIOFSDirectory for the named location and the default lock factory.
//        ///
//        /// </summary>
//        /// <param name="path">the path of the directory
//        /// </param>
//        /// <throws>  IOException </throws>
//        [System.Obsolete("Use the constructor that takes a DirectoryInfo, this will be removed in the 3.0 release")]
//        public NIOFSDirectory(FileInfo path):base(new DirectoryInfo(path.FullName), null)
//        {
//        }

//        /// <summary>Create a new NIOFSDirectory for the named location and the default lock factory.
//        ///
//        /// </summary>
//        /// <param name="path">the path of the directory
//        /// </param>
//        /// <throws>  IOException </throws>
//        public NIOFSDirectory(DirectoryInfo path) : base(path, null)
//        {
//        }

//        // back compatibility so FSDirectory can instantiate via reflection
//        /// <deprecated>
//        /// </deprecated>
//        [Obsolete]
//        internal NIOFSDirectory()
//        {
//        }

//        /// <summary>Creates an IndexInput for the file with the given name. </summary>
//        public override IndexInput OpenInput(string name, int bufferSize)
//        {
//            EnsureOpen();
//            return new NIOFSIndexInput(new FileInfo(Path.Combine(GetFile().FullName, name)), bufferSize, GetReadChunkSize());
//        }

//        /// <summary>Creates an IndexOutput for the file with the given name. </summary>
//        public override IndexOutput CreateOutput(string name)
//        {
//            InitOutput(name);
//            return new SimpleFSDirectory.SimpleFSIndexOutput(new FileInfo(Path.Combine(directory.FullName, name)));
//        }

//        public /*protected internal*/ class NIOFSIndexInput:SimpleFSDirectory.SimpleFSIndexInput
//        {
//            private MemoryStream byteBuf; // wraps the buffer for NIO

//            private byte[] otherBuffer;
//            private MemoryStream otherByteBuf;

//            internal BinaryReader channel;

//            /// <deprecated> Please use ctor taking chunkSize
//            /// </deprecated>
//            [Obsolete("Please use ctor taking chunkSize")]
//            public NIOFSIndexInput(FileInfo path, int bufferSize):this(path, bufferSize, FSDirectory.DEFAULT_READ_CHUNK_SIZE)
//            {
//            }

//            public NIOFSIndexInput(FileInfo path, int bufferSize, int chunkSize):base(path, bufferSize, chunkSize)
//            {
//                channel = (BinaryReader) file;
//            }

//            protected internal override void  NewBuffer(byte[] newBuffer)
//            {
//                base.NewBuffer(newBuffer);
//                // {{Aroush-2.9}} byteBuf = ByteBuffer.wrap(newBuffer);
//                System.Diagnostics.Debug.Fail("Port issue:", "byteBuf = ByteBuffer.wrap(newBuffer)"); // {{Aroush-2.9}}
//            }

//            public override void  Close()
//            {
//                if (!isClone && file.isOpen)
//                {
//                    // Close the channel & file
//                    try
//                    {
//                        channel.Close();
//                    }
//                    finally
//                    {
//                        file.Close();
//                    }
//                }
//            }

//            public override void  ReadInternal(byte[] b, int offset, int len)
//            {
//                MemoryStream bb;

//                // Determine the ByteBuffer we should use
//                if (b == buffer && 0 == offset)
//                {
//                    // Use our own pre-wrapped byteBuf:
//                    System.Diagnostics.Debug.Assert(byteBuf != null);
//                    byteBuf.Position = 0;
//                    byteBuf.Capacity = len;
//                    bb = byteBuf;
//                }
//                else
//                {
//                    if (offset == 0)
//                    {
//                        if (otherBuffer != b)
//                        {
//                            // Now wrap this other buffer; with compound
//                            // file, we are repeatedly called with its
//                            // buffer, so we wrap it once and then re-use it
//                            // on subsequent calls
//                            otherBuffer = b;
//                            // otherByteBuf = ByteBuffer.wrap(b); {{Aroush-2.9}}
//                            System.Diagnostics.Debug.Fail("Port issue:", "otherByteBuf = ByteBuffer.wrap(b)"); // {{Aroush-2.9}}
//                        }
//                        else
//                            otherByteBuf.Position = 0;
//                        otherByteBuf.Capacity = len;
//                        bb = otherByteBuf;
//                    }
//                    else
//                    {
//                        // Always wrap when offset != 0
//                        bb = null; // bb = ByteBuffer.wrap(b, offset, len); {{Aroush-2.9}}
//                        System.Diagnostics.Debug.Fail("Port issue:", "bb = ByteBuffer.wrap(b, offset, len)"); // {{Aroush-2.9}}
//                    }
//                }

//                int readOffset = (int) bb.Position;
//                int readLength = bb.Capacity - readOffset;
//                System.Diagnostics.Debug.Assert(readLength == len);

//                long pos = GetFilePointer();

//                try
//                {
//                    while (readLength > 0)
//                    {
//                        int limit;
//                        if (readLength > chunkSize)
//                        {
//                            // LUCENE-1566 - work around JVM Bug by breaking
//                            // very large reads into chunks
//                            limit = readOffset + chunkSize;
//                        }
//                        else
//                        {
//                            limit = readOffset + readLength;
//                        }
//                        bb.Capacity = limit;
//                        int i = -1; // int i = channel.Read(bb, pos, limit); // {{Aroush-2.9}} must read from 'channel' into 'bb'
//                        System.Diagnostics.Debug.Fail("Port issue:", "channel.Read(bb, pos, limit)"); // {{Aroush-2.9}}
//                        if (i == - 1)
//                        {
//                            throw new IOException("read past EOF");
//                        }
//                        pos += i;
//                        readOffset += i;
//                        readLength -= i;
//                    }
//                }
//                catch (System.OutOfMemoryException e)
//                {
//                    // propagate OOM up and add a hint for 32bit VM Users hitting the bug
//                    // with a large chunk size in the fast path.
//                    System.OutOfMemoryException outOfMemoryError = new System.OutOfMemoryException("OutOfMemoryError likely caused by the Sun VM Bug described in " + "https://issues.apache.org/jira/browse/LUCENE-1566; try calling FSDirectory.setReadChunkSize " + "with a a value smaller than the current chunk size (" + chunkSize + ")", e);
//                    throw outOfMemoryError;
//                }
//            }
//        }
//    }
//}