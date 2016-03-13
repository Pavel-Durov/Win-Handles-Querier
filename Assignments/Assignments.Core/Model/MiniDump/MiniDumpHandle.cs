﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.WinApi;
using Assignments.Core.Handlers;

namespace Assignments.Core.Model.MiniDump
{
    public enum MiniDumpHandleType
    {
        NONE,
        THREAD,
        MUTEX1,
        MUTEX2,
        PROCESS1,
        PROCESS2,
        EVENT,
        SECTION,
        TYPE_MAX
    }

    public class MiniDumpHandle
    {

        public MiniDumpHandle(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
        }

        public MiniDumpHandle(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2 handleDescriptor)
        {
            Handle = handleDescriptor.Handle;
            HandleCount = handleDescriptor.HandleCount;
            ObjectNameRva = handleDescriptor.ObjectNameRva;
            PointerCount = handleDescriptor.PointerCount;
            TypeNameRva = handleDescriptor.TypeNameRva;
            Attributes = handleDescriptor.Attributes;
            GrantedAccess = handleDescriptor.GrantedAccess;
            ObjectInfoRva = handleDescriptor.ObjectInfoRva;
        }

        public MiniDumpHandle(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2 handleDescriptor, string objectName, string typeName) : this(handleDescriptor)
        {
            this.ObjectName = objectName;
            this.TypeName = typeName;
        }

        public string ObjectName { get; private set; }
        public string TypeName { get; private set; }


        public ulong Handle { get; private set; }
        public uint HandleCount { get; private set; }

        /// <summary>
        /// An RVA to a MINIDUMP_STRING structure that specifies the object name of the handle. This member can be 0.
        /// </summary>
        public uint ObjectNameRva { get; private set; }
        public uint PointerCount { get; private set; }
        public uint TypeNameRva { get; private set; }
        public uint Attributes { get; private set; }
        public uint GrantedAccess { get; private set; }

        public MiniDumpHandleType Type { get; set; }
        /// <summary>
        /// An RVA to a MINIDUMP_HANDLE_OBJECT_INFORMATION structure that specifies object-specific information. This member can be 0 if there is no extra information.
        /// </summary>
        public uint ObjectInfoRva { get; private set; }
        public bool HasObjectInfo{ get { return ObjectInfoRva > 0; } }

        public MiniDumpHandleInfo HandleInfo { get; private set; }
        public uint OwnerProcessId { get; internal set; }
        public uint OwnerThreadId { get; internal set; }
        public MutexUnknownFields MutexUnknown { get; internal set; }
        public string Name { get; internal set; }

        internal void AddInfo(DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION info, string infoName)
        {
            HandleInfo = new MiniDumpHandleInfo(info);
        }
    }

    public class MutexUnknownFields
    {
        public object Field1 { get; internal set; }
        public object Field2 { get; internal set; }
    }
}
