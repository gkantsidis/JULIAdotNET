﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;

//Written by Johnathan Bizzano 
namespace JULIAdotNET
{

    [StructLayout(LayoutKind.Sequential)]
    public struct JLVal {
        internal readonly IntPtr ptr;

        public JLVal(object o) : this(DotNotType(o).ptr) { }
        public JLVal(IntPtr ptr) => this.ptr = ptr;
        public unsafe JLVal(void* l) : this(JuliaCalls.jl_box_voidpointer(new IntPtr(l))) { }
        public JLVal(long l) : this(JuliaCalls.jl_box_int64(l)) { }
        public JLVal(int l) : this(JuliaCalls.jl_box_int32(l)) { }
        public JLVal(short l) : this(JuliaCalls.jl_box_int16(l)) { }
        public JLVal(sbyte l) : this(JuliaCalls.jl_box_int8(l)) { }
        public JLVal(ulong l) : this(JuliaCalls.jl_box_uint64(l)) { }
        public JLVal(uint l) : this(JuliaCalls.jl_box_uint32(l)) { }
        public JLVal(ushort l) : this(JuliaCalls.jl_box_uint16(l)) { }
        public JLVal(byte l) : this(JuliaCalls.jl_box_uint8(l)) { }
        public JLVal(char l) : this(JuliaCalls.jl_box_int32(l)) { }
        public JLVal(bool l) : this(JuliaCalls.jl_box_bool(l)) { }
        public JLVal(double l) : this(JuliaCalls.jl_box_float64(l)) { }
        public JLVal(float l) : this(JuliaCalls.jl_box_float32(l)) { }
        public JLVal(string s) : this(JuliaCalls.jl_cstr_to_string(s)) { }
        public JLVal(Array a) : this(new JLArray(a)) { }
        public JLVal(Type t) : this(NativeSharp.GetType(t)) { }
        public JLVal(ConstructorInfo ci) : this(NativeSharp.GetConstructor(ci)) { }
        public JLVal(MethodInfo mi) : this(NativeSharp.GetMethod(mi)) { }
        public JLVal(FieldInfo fi) : this(NativeSharp.GetField(fi)) { }


        public static implicit operator IntPtr(JLVal value) => value.ptr;
        public static implicit operator JLVal(IntPtr ptr) => new JLVal(ptr);
        public static implicit operator JLVal(long l) => new JLVal(l);
        public static implicit operator JLVal(ulong l) => new JLVal(l);
        public static implicit operator JLVal(int l) => new JLVal(l);
        public static implicit operator JLVal(uint l) => new JLVal(l);
        public static implicit operator JLVal(short l) => new JLVal(l);
        public static implicit operator JLVal(ushort l) => new JLVal(l);
        public static implicit operator JLVal(string l) => new JLVal(l);
        public static implicit operator JLVal(double l) => new JLVal(l);
        public static implicit operator JLVal(float l) => new JLVal(l);
        public static implicit operator JLVal(char l) => new JLVal(l);
        public static implicit operator JLVal(bool l) => new JLVal(l);
        public static implicit operator JLVal(byte l) => new JLVal(l);
        public static implicit operator JLVal(sbyte l) => new JLVal(l);
        public static implicit operator JLVal(Array l) => new JLVal(l);
        public static implicit operator JLVal(Type l) => new JLVal(l);
        public static implicit operator JLVal(MethodInfo l) => new JLVal(l);
        public static implicit operator JLVal(ConstructorInfo l) => new JLVal(l);
        public static implicit operator JLVal(FieldInfo l) => new JLVal(l);

        public static explicit operator long(JLVal value) => value.UnboxInt64();
        public static explicit operator ulong(JLVal value) => value.UnboxUInt64();
        public static explicit operator int(JLVal value) => value.UnboxInt32();
        public static explicit operator uint(JLVal value) => value.UnboxUInt32();
        public static explicit operator short(JLVal value) => value.UnboxInt16();
        public static explicit operator ushort(JLVal value) => value.UnboxUInt16();
        public static explicit operator byte(JLVal value) => value.UnboxUInt8();
        public static explicit operator sbyte(JLVal value) => value.UnboxInt8();
        public static explicit operator string(JLVal value) => value.UnboxString();
        public static explicit operator char(JLVal value) => value.UnboxChar();
        public static explicit operator bool(JLVal value) => value.UnboxBool();
        public static explicit operator double(JLVal value) => value.UnboxFloat64();
        public static explicit operator float(JLVal value) => value.UnboxFloat32();
        public static explicit operator Array(JLVal value) => value.UnboxArray();
        public static explicit operator Type(JLVal value) => value.UnboxSharpType();
        public static explicit operator FieldInfo(JLVal value) => value.UnboxFieldInfo();
        public static explicit operator MethodInfo(JLVal value) => value.UnboxMethodInfo();
        public static explicit operator ConstructorInfo(JLVal value) => value.UnboxConstructorInfo();

        public static bool operator ==(JLVal value1, JLVal value2) => JLFun.IsEqualF.Invoke(value1, value2).UnboxBool();
        public static bool operator !=(JLVal value1, JLVal value2) => JLFun.IsNEqualF.Invoke(value1, value2).UnboxBool();
        public static bool operator >(JLVal value1, JLVal value2) => JLFun.IsGreaterF.Invoke(value1, value2).UnboxBool();
        public static bool operator <(JLVal value1, JLVal value2) => JLFun.IsLessF.Invoke(value1, value2).UnboxBool();
        public static bool operator >=(JLVal value1, JLVal value2) => JLFun.IsGreaterOrEqualF.Invoke(value1, value2).UnboxBool();
        public static bool operator <=(JLVal value1, JLVal value2) => JLFun.IsLessOrEqualF.Invoke(value1, value2).UnboxBool();

        public override bool Equals(object o) => o is JLVal && ((JLVal)o).ptr == ptr;
        public override int GetHashCode() => JLFun.HashCodeF.Invoke(this).UnboxInt32();

        public unsafe long SizeOf { get => JLType.JLAny.IsType(this) ? sizeof(void*) : JLFun.SizeOfF.Invoke(this).UnboxInt64(); }
        public JLType ElType { get => JLFun.ElTypeF.Invoke(this); }
        public JLType Type { get => new JLType(JLFun.TypeOfF.Invoke(this)); }
        public SizeT Length { get => JLFun.LengthF.Invoke(this); }
        public bool IsFloat64 { get => JLType.JLFloat64.IsType(this); }
        public bool IsFloat32 { get => JLType.JLFloat32.IsType(this); }
        public bool IsInt64 { get => JLType.JLInt64.IsType(this); }
        public bool IsInt32 { get => JLType.JLInt32.IsType(this); }
        public bool IsInt16 { get => JLType.JLInt16.IsType(this); }
        public bool IsInt8 { get => JLType.JLInt8.IsType(this); }
        public bool IsUInt64 { get => JLType.JLUInt64.IsType(this); }
        public bool IsUInt32 { get => JLType.JLUInt32.IsType(this); }
        public bool IsUInt16 { get => JLType.JLUInt16.IsType(this); }
        public bool IsUInt8 { get => JLType.JLUInt8.IsType(this); }
        public bool IsString { get => JLType.JLString.IsType(this); }
        public bool IsBool { get => JLType.JLBool.IsType(this); }
        public bool IsChar { get => JLType.JLChar.IsType(this); }
        public bool IsPtr { get => JLType.JLPtr.IsType(this); }
        public bool IsArray { get => JLType.JLArray.IsType(this); }
        public bool IsSharpObject { get => JLType.SharpObject.IsType(this); }
        public JLVal Size { get => JLFun.SizeF.Invoke(this); }
        public bool IsNull { get => ptr == IntPtr.Zero || JLType.JLNothing.IsType(this); }
        public bool IsSharpType { get => JLType.SharpType.IsType(this); }

        public object Value {
            get {
                if (IsFloat64)
                    return UnboxFloat64();
                if (this.IsFloat32)
                    return UnboxFloat32();
                if (this.IsString)
                    return UnboxString();
                if (this.IsInt64)
                    return UnboxInt64();
                if (this.IsInt32)
                    return UnboxInt32();
                if (this.IsInt16)
                    return UnboxInt16();
                if (this.IsInt8)
                    return UnboxInt8();
                if (this.IsBool)
                    return UnboxBool();
                if (this.IsUInt64)
                    return UnboxUInt64();
                if (this.IsUInt32)
                    return UnboxUInt32();
                if (this.IsUInt16)
                    return UnboxUInt16();
                if (this.IsUInt8)
                    return UnboxUInt8();
                if (this.IsSharpObject)
                    return UnboxSharpObject();
                if (this.IsSharpType)
                    return UnboxSharpType();
                if (this.IsArray)
                    return UnboxArray();
                if (this.IsChar)
                    return UnboxChar();
                if (this.IsPtr)
                    return UnboxPtr();
                if (this.IsNull)
                    return null;
                return this;
            }
        }
        

        public long UnboxInt64() => JuliaCalls.jl_unbox_int64(this);
        public int UnboxInt32() => JuliaCalls.jl_unbox_int32(this);
        public short UnboxInt16() => JuliaCalls.jl_unbox_int16(this);
        public sbyte UnboxInt8() => JuliaCalls.jl_unbox_int8(this);
        public bool UnboxBool() => JuliaCalls.jl_unbox_bool(this);
        public ulong UnboxUInt64() => JuliaCalls.jl_unbox_uint64(this);
        public uint UnboxUInt32() => JuliaCalls.jl_unbox_uint32(this);
        public ushort UnboxUInt16() => JuliaCalls.jl_unbox_uint16(this);
        public byte UnboxUInt8() => JuliaCalls.jl_unbox_uint8(this);
        public double UnboxFloat64() => JuliaCalls.jl_unbox_float64(this);
        public float UnboxFloat32() => JuliaCalls.jl_unbox_float32(this);
        public char UnboxChar() => (char) JuliaCalls.jl_unbox_int32(this);
        public IntPtr UnboxPtr() => JuliaCalls.jl_unbox_voidpointer(this);
        public string UnboxString() => Julia.UnboxString(this);
        public object UnboxSharpObject() => JLFun._UnboxsharpobjectF.Invoke(this);
        public Type UnboxSharpType() => (Type) UnboxSharpObject();
        public Array UnboxArray() => new JLArray(this).UnboxArray();
        public FieldInfo UnboxFieldInfo() => (FieldInfo) UnboxSharpObject();
        public ConstructorInfo UnboxConstructorInfo() => (ConstructorInfo) UnboxSharpObject();
        public MethodInfo UnboxMethodInfo() => (MethodInfo)UnboxSharpObject();

        public JLVal Convert(JLType t) => JLFun.ConvertF.Invoke(t, this);
        public JLVal GetFieldValue(JLSym fieldName) => JLFun.GetFieldF.Invoke(this, fieldName);
        public JLVal SetFieldValue(JLSym fieldName, JLVal v) => JLFun.SetField_F.Invoke(this, fieldName, v);
        

        public override string ToString() => JLFun.StringF.Invoke(this).UnboxString();
        public void Println() => JLFun.PrintlnF.Invoke(this);
        public void Print() => JLFun.PrintF.Invoke(this);
        public void Add(JLVal val) => JLFun.Push_F.Invoke(this, val);
        public void Remove(JLVal val) => JLFun.Delete_F.Invoke(this, val);
        public void RemoveAt(JLVal idx) => JLFun.Deleteat_F.Invoke(this, idx);
        public void RemoveAt(JLRange range) => JLFun.Deleteat_F.Invoke(this, range);
        public void Clear() => JLFun.Empty_F.Invoke(this);
        public void Write(JLVal v) => JLFun.WriteF.Invoke(this, v);

        public JLVal this[JLVal idx] {
            get => JLFun.GetIndexF.Invoke(this, idx);
            set => setEl(idx, value);
        }

        internal void setEl(long idx, JLVal val) => JLFun.SetIndex_F.Invoke(this, val, idx);
        internal void setEl(JLArray idxs, JLVal val) => JLFun.SetIndex_F.Invoke(this, val, idxs);

        public static JLVal DotNotType(object o)
        {
            if (JLType.IsPointerType(o))
                return new JLVal((IntPtr) o);
            if (o is long)
                return new JLVal((long) o);
            if (o is ulong)
                return new JLVal((ulong) o);
            if (o is int)
                return new JLVal((int) o);
            if (o is uint)
                return new JLVal((uint) o);
            if (o is short)
                return new JLVal((short) o);
            if (o is ushort)
                return new JLVal((ushort) o);
            if (o is sbyte)
                return new JLVal((sbyte) o);
            if (o is byte)
                return new JLVal((byte) o);
            if (o is double)
                return new JLVal((double) o);
            if (o is float)
                return new JLVal((float) o);
            if (o is bool)
                return new JLVal((bool) o);
            if (o is string)
                return new JLVal((string) o);
            if (o is Array)
                return new JLVal((Array) o);
            if (o is IntPtr)
                return Julia.BoxPtr((IntPtr) o);
            if (o is Type)
                return new JLVal((Type) o);
            if (o is MethodInfo)
                return new JLVal((MethodInfo) o);
            if (o is ConstructorInfo)
                return new JLVal((ConstructorInfo) o);
            if (o is FieldInfo)
                return new JLVal((FieldInfo)o);
            return NativeSharp.BoxObject(o);
        }
    }
}
