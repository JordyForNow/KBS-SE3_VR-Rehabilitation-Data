﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MotionRecognition
{
    [Serializable]
    public class Motion3DImage : IImage
    {
        public int size;
        public BitModulator[,] top, side;

        public Motion3DImage(int s = 500)
        {
            SetSize(s);
        }

        public Motion3DImage(ref Table<Measurement> t) : this()
        {
            LoadImage(ref t);
        }

        public void LoadImage(ref Table<Measurement> t)
        {
            Vec3 vecMin = new Vec3();
            Vec3 vecMax = new Vec3();
            foreach (var s in t.samples)
            {
                foreach (var m in s.sampleData)
                {
                    vecMin.x = m.pos.x < vecMin.x ? m.pos.x : vecMin.x;
                    vecMin.y = m.pos.y < vecMin.y ? m.pos.y : vecMin.y;
                    vecMin.z = m.pos.z < vecMin.z ? m.pos.z : vecMin.z;

                    vecMax.y = m.pos.y > vecMax.y ? m.pos.y : vecMax.y;
                    vecMax.x = m.pos.x > vecMax.x ? m.pos.x : vecMax.x;
                    vecMax.z = m.pos.z > vecMax.z ? m.pos.z : vecMax.z;
                }
            }
            int x, y, z;
            foreach (var sample in t.samples)
            {
                for (int i = 0; i < sample.sampleData.Count; i++)
                {
                    x = (int)Math.Round(Remap(sample.sampleData[i].pos.x, vecMin.x, vecMax.x, 0, size - 1));
                    y = (int)Math.Round(Remap(sample.sampleData[i].pos.y, vecMin.y, vecMax.y, 0, size - 1));
                    z = (int)Math.Round(Remap(sample.sampleData[i].pos.z, vecMin.z, vecMax.z, 0, size - 1));
                    if (this.top[x, y] == null) this.top[x, y] = new BitModulator();
                    this.top[x, y].SetIndex(i, true);
                    if (this.side[z, y] == null) this.side[z, y] = new BitModulator();
                    this.side[z, y].SetIndex(i, true);
                }
            }
        }

        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public void Serialize(string filePath = "./data")
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = File.Create(filePath))
            {
                AddText(fs, size + Environment.NewLine);
                writeBitModulator(fs, top);
                AddText(fs, "|" + Environment.NewLine);
                writeBitModulator(fs, side);
            }
        }

        public Motion3DImage DeSerialize(string filePath = "./data")
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            //Index 0 = size, 1 = values for top, 2 = values for side
            int index = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);

            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split(",");

                switch (index)
                {
                    case 0:
                        this.size = int.Parse(line);
                        index = 1;
                        break;
                    case 1:
                        //If seperator, switch to index 2 
                        if (line == "|") {
							index = 2;
							break;
						}

                        this.top[int.Parse(values[0]), int.Parse(values[1])] = new BitModulator(values[2]);
                        break;
                    case 2:
                        this.side[int.Parse(values[0]), int.Parse(values[1])] = new BitModulator(values[2]);
                        break;
                }
            }

            file.Close();

            return this;
        }

        private BitModulator[,] createBitModulator(string[] arr)
        {
            BitModulator[,] bm = new BitModulator[size, size];

            foreach (string val in arr)
            {
                string v = Regex.Replace(val, "({|})", "");

                string[] values = v.Split(",");
                int x = int.Parse(values[0]);
                int y = int.Parse(values[1]);

                bm[x, y] = new BitModulator();
                bm[x, y].SetVal(uint.Parse(values[2]));
            }

            return bm;
        }

        private void writeBitModulator(FileStream fs, BitModulator[,] b)
        {
            for (uint x = 0; x < size; x++)
            {
                for (uint y = 0; y < size; y++)
                {
                    if (b[x, y] != null)
                        AddText(fs, $"{x},{y},{b[x, y].GetVal()} {Environment.NewLine}");
                }
            }
        }

        private void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Motion3DImage)obj;

            if (size != other.size) return false;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (top[x, y] == null && other.top[x, y] == null) // if both cells are not set they are equal
                    {
                        continue;
                    }
                    else if ((top[x, y] == null && other.top[x, y] != null) || (top[x, y] != null && other.top[x, y] == null)) // if either cell is not set they are not equal
                    {
                        return false;
                    }
                    else if (top[x, y].Equals(other.top[x, y])) // if equals they are equal
                    {
                        continue;
                    }
                    else // the cells are not equal
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void SetSize(int s)
        {
            this.size = s;
            top = new BitModulator[size, size];
            side = new BitModulator[size, size];
        }
    }
}
