﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TraceMeGL
{
	public class ObjectManager
	{
		private class ArrayIndexCombination
		{
			public int index;
			public Array data;
		}

		private class TypeIndicesInfo
		{
			public int bufferUBO = -1;
			public int bufferIndex = -1;
			public int arrayLengthLocation = -1;
			public int arrayLocation = -1;
			public Dictionary<FieldInfo, int> fieldArrayIndices = new Dictionary<FieldInfo, int>();
		}

		public static readonly Lazy<ObjectManager> Instance = new Lazy<ObjectManager>(() => new ObjectManager());

		public Shader VertexShader { get; private set; }
		public Shader FragmentShader { get; private set; }

		public ShaderProgram Program { get; private set; }

		private Dictionary<Type, ArrayIndexCombination> objects = new Dictionary<Type, ArrayIndexCombination>();
		private Dictionary<Type, TypeIndicesInfo> bufferIndices = new Dictionary<Type, TypeIndicesInfo>();

		private Dictionary<Type, string> typeToGLSL = new Dictionary<Type, string>()
		{
			{typeof(Vector2), "vec2"},
			{typeof(Vector3), "vec3"},
			{typeof(Vector4), "vec4"},
			{typeof(float), "float"},
			{typeof(int), "int"}
		};

		private int eyePositionLocation;
		private int clearColorLocation;
		private int widthLocation;
		private int heightLocation;

		private ObjectManager()
		{
			//Generating shaders  
			var x = from type in Assembly.GetExecutingAssembly().GetTypes()
					let attrs = type.GetCustomAttributes()
					let attr1 = attrs.FirstOrDefault(a => a is IntersectionAttribute) as IntersectionAttribute
					//let attr2 = attrs.FirstOrDefault(a => a is StructDataAttribute) as StructDataAttribute
					where attr1 != null// && attr2 != null
					select new { type, attr1 };

			List<string> structDefs = new List<string>();
			List<string> arrDefs = new List<string>();
			List<string> intersectDefs = new List<string>();
			List<string> renderCycles = new List<string>();
			List<string> arrayLengths = new List<string>();

			foreach (var u in x)
			{
				bufferIndices.Add(u.type, new TypeIndicesInfo() { bufferIndex = bufferIndices.Count });
				string fields = "";
				foreach (var f in u.type.GetFields())
				{
					fields += string.Format("{0} {1}; \n", typeToGLSL[f.FieldType], f.Name);
				}
				structDefs.Add(string.Format("struct {0}\n{{\n{1}}};", u.type.Name, fields));
				//arrDefs.Add(string.Format("layout(std140) uniform {0}_uniform {{ {0} {0}_arr[MAX_OBJECT_ARRAY_SIZE]; }};\n", u.type.Name));
				//arrDefs.Add(string.Format("uniform {0} {0}_arr[MAX_OBJECT_ARRAY_SIZE];\n", u.type.Name));
				arrDefs.Add(u.type.GetFields().Select(i => string.Format("uniform {0} {1}_{2}_arr[MAX_OBJECT_ARRAY_SIZE];\n", typeToGLSL[i.FieldType], u.type.Name, i.Name)).Aggregate((a, b) => a + b));
				intersectDefs.Add(string.Format("Hit Intersection_{0}({0} obj, Lay lay) {{ {1} }}\n", u.type.Name, u.attr1.Text()));
				renderCycles.Add(string.Format(@"for(int i = 0; i < {0}_arr_length; i++) 
{{
	{0} obj = {0}({1});
	Hit hit = Intersection_{0}(obj, lastLay); 
	if (hit.t < min && hit.t > 0) 
	{{
		min = hit.t; minHit = hit;
	}} 
}}", u.type.Name, u.type.GetFields().Select(i => string.Format("{0}_{1}_arr[i], ", u.type.Name, i.Name)).Aggregate((a, b) => a + b).TrimEnd(' ', ',')));
				arrayLengths.Add(string.Format("uniform int {0}_arr_length = -1;\n", u.type.Name));
			}
			string pixelText = string.Format(@"#version 330

struct Hit
{{
	float t;
	vec4 color;
	vec3 normal;
	float reflection;
}};

struct Lay
{{
	vec3 point;
	vec3 dir;
}};

{0}

const int MAX_OBJECT_ARRAY_SIZE = 100;
const int RENDER_DEPTH = 3;

{1}

uniform vec3 eyePosition;
uniform vec4 clearColor = vrc4(0, 1, 1, 1);
uniform int width;
uniform int height;

{4}

{2}

vec4 shuffle(vec4 a, vec4 b, float r)
{{
	return sqrt(((1 - r) * a.xyzw * a.xyzw + r * b.xyzw * b.xyzw ) / 2);
}}

vec4 Render(Lay lay)
{{
	Lay lastLay = lay;
	vec4 result = clearColor;
	float min = 1.0 / 0.0;
	Hit minHit = Hit(-1.0, result, vec3(0, 0, 0), 0);

{3}

	if (minHit.t < 0.0)
		return result;
	Hit lastHit = minHit;
	
	for (int i = 0; i < RENDER_DEPTH; i++)
	{{
		if (lastHit.reflection > 0)
		{{
			min = 1.0 / 0.0;
			minHit = Hit(-1.0, result, vec3(0, 0, 0), 0);

			{3}

			if (minHit.t <= 0)
			{{
				break;
			}}
			
			vec3 intersection = lastLay.point + lastLay.dir * lastHit.t;
			lastLay = Lay(intersection, reflect(lastLay.dir, lastHit.normal));

			lastHit.color = shuffle(minHit.color, lastHit.color, lastHit.reflection);
			lastHit.t = minHit.t;
			lastHit.normal = minHit.normal;
			lastHit.reflection = minHit.reflection;
		}}
	}}

	return lastHit.color;
}}

varying out vec4 resultColor;
void main()
{{
	vec3 point = vec3(gl_FragCoord.x / width * 2 - 1, (gl_FragCoord.y * 2 - height) / width, 0);
	vec3 dir = normalize(point - eyePosition);
	Lay lay = Lay(point, dir);
	resultColor = Render(lay);
	//resultColor = clearColor;
}}


		",	 structDefs.Count > 0 ? structDefs.Aggregate((a, b) => a + '\n' + b) : "", 
			 arrDefs.Count > 0 ? arrDefs.Aggregate((a, b) => a + b)		  : "", 
			 intersectDefs.Count > 0 ? intersectDefs.Aggregate((a, b) => a + b) : "",
			 renderCycles.Count > 0 ? renderCycles.Aggregate((a, b) => a + '\n' + b) : "",
			 arrayLengths.Count > 0 ? arrayLengths.Aggregate((a, b) => a + b) : "");

			//Shader compilation
			FragmentShader = new Shader(pixelText, ShaderType.FragmentShader);
			VertexShader = new Shader(File.ReadAllText("basicVertexShader.txt"), ShaderType.VertexShader);
			Program = new ShaderProgram(FragmentShader, VertexShader);

			//data indices filling
			foreach (var p in bufferIndices)
			{
				if (p.Value.bufferIndex == -1)
					throw new Exception("Huston, we have a problem!");

				//var dataLength = objects[p.Key].data.Length;

				//p.Value.bufferUBO = GL.GenBuffer();

				//GL.BindBuffer(BufferTarget.UniformBuffer, p.Value.bufferUBO);
				//GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)(Marshal.SizeOf(p.Key) * dataLength), (IntPtr)null, BufferUsageHint.StreamDraw);
				//GL.BindBufferRange(BufferRangeTarget.UniformBuffer, p.Value.bufferIndex, p.Value.bufferUBO, (IntPtr)0, (IntPtr)(Marshal.SizeOf(p.Key) * dataLength));

				//var UniformBlockLocation = GL.GetUniformBlockIndex(Program.ID, string.Format("{0}_uniform", p.Key.Name));
				//GL.UniformBlockBinding(Program.ID, UniformBlockLocation, p.Value.bufferIndex);

				//p.Value.arrayLocation = GL.GetUniformLocation(Program.ID, string.Format("{0}_arr", p.Key.Name));
				p.Value.arrayLengthLocation = GL.GetUniformLocation(Program.ID, string.Format("{0}_arr_length", p.Key.Name));
				foreach (var i in p.Key.GetFields())
				{
					p.Value.fieldArrayIndices.Add(i, GL.GetUniformLocation(Program.ID, string.Format("{0}_{1}_arr", p.Key.Name, i.Name)));
				}
			}

			eyePositionLocation = GL.GetUniformLocation(Program.ID, "eyePosition");
			clearColorLocation = GL.GetUniformLocation(Program.ID, "clearColor");
			widthLocation = GL.GetUniformLocation(Program.ID, "width");
			heightLocation = GL.GetUniformLocation(Program.ID, "height");
		}

		public Holder<T> CreateObject<T>() where T : struct
		{
			Type type = typeof(T);
			ArrayIndexCombination objectArray = null;
			if (!objects.TryGetValue(type, out objectArray))
			{
				var createdData = new T[4];
				objectArray = new ArrayIndexCombination() { index = -1, data = createdData };
				objects.Add(type, objectArray);
			}

			objectArray.index++;
			if (objectArray.index >= objectArray.data.Length - 1)
				objectArray.data = Extend<T>((T[])objectArray.data);

			Holder<T> result = new Holder<T>((T[])objectArray.data, objectArray.index);
			return result;
		}

		private T[] Extend<T>(T[] data)
		{
			T[] newData = new T[data.Length * 2];
			Array.Copy(data, newData, data.Length);
			return newData;
		}

		byte[] getBytes(object str)
		{
			int size = Marshal.SizeOf(str);
			byte[] arr = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(str, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);

			return arr;
		}

		public void FlushObjects()
		{
			GL.UseProgram(Program.ID);
			foreach (var p in objects)
			{
				//int bufferUBO = bufferIndices[p.Key].bufferUBO;
				//int bufferIndex = bufferIndices[p.Key].bufferIndex;
				GL.Uniform1(bufferIndices[p.Key].arrayLengthLocation, p.Value.index + 1);
				//GL.BindBuffer(BufferTarget.UniformBuffer, bufferUBO);
				//GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)(Marshal.SizeOf(p.Key) * p.Value.data.Length), (IntPtr)null, BufferUsageHint.StreamDraw);
				//GL.BindBufferRange(BufferRangeTarget.UniformBuffer, bufferIndex, bufferUBO, (IntPtr)0, (IntPtr)(Marshal.SizeOf(p.Key) * p.Value.data.Length));

				//GCHandle pinner = GCHandle.Alloc(p.Value.data, GCHandleType.Pinned);
				//GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)0, (IntPtr)(Marshal.SizeOf(p.Key) * p.Value.data.Length), pinner.AddrOfPinnedObject());
				//pinner.Free();

				//GL.BindBuffer(BufferTarget.UniformBuffer, 0);

				foreach (var i in bufferIndices[p.Key].fieldArrayIndices)
				{
					var x = p.Value.data.Cast<object>().Select(o => i.Key.GetValue(o)).ToArray();
					switch (typeToGLSL[i.Key.FieldType])
					{
						case "float":
							GL.Uniform1(i.Value, x.Length, x.Cast<float>().ToArray());
							break;
						case "int":
							GL.Uniform1(i.Value, x.Length, x.Cast<int>().ToArray());
							break;
						case "vec2":
							GL.Uniform2(i.Value, x.Length, x.Cast<Vector2>().Select(v => new[] { v.X, v.Y }).SelectMany(a => a).ToArray());
							break;
						case "vec3":
							GL.Uniform3(i.Value, x.Length, x.Cast<Vector3>().Select(v => new[] { v.X, v.Y, v.Z }).SelectMany(a => a).ToArray());
							break;
						case "vec4":
							GL.Uniform4(i.Value, x.Length, x.Cast<Vector4>().Select(v => new[] { v.X, v.Y, v.Z, v.W }).SelectMany(a => a).ToArray());
							break;
						default:
							break;
					}
				}
			}
		}

		public void CalculateEye(int width, int height, float fov)
		{
			double cos = Math.Cos(fov);
			double h = Math.Sqrt((1 + cos) / (1 - cos));
			var eye = new Vector3(0, 0, (float)-h);
			GL.UseProgram(Program.ID);
			GL.Uniform3(eyePositionLocation, eye);
			GL.Uniform1(widthLocation, width);
			GL.Uniform1(heightLocation, height);
		}

		public void SetClearColor(Vector4 color)
		{
			GL.UseProgram(Program.ID);
			GL.Uniform4(clearColorLocation, color);
		}
	}
}
