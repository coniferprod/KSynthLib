using System.Text;
using System.Collections.Generic;
using System.IO;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents common settings for a K5000S multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiCommon : ISystemExclusiveData
    {
        public static readonly int DataSize = 54;

        public EffectAlgorithm EffectAlgorithm; // 1~4 (in SysEx 0~3)
        public ReverbSettings Reverb;
        public EffectSettings Effect1;
        public EffectSettings Effect2;
        public EffectSettings Effect3;
        public EffectSettings Effect4;
        public GEQSettings GEQ;

        /// <value>
        /// Name of the multi/combi patch.
        /// </value>
        public PatchName Name;

        public PositiveLevel Volume;

        public bool[] IsSectionMuted;  // 0=muted, 1=not muted - store the inverse here

        public EffectControl EffectControl1;
        public EffectControl EffectControl2;

        public MultiCommon()
        {
            this.Name = new PatchName("NewMulti");


        }

        public MultiCommon(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, false))
	        {
                byte effectAlgorithm = (byte) ms.ReadByte();
                // Seems like the K5000 might set the top bits of this, so mask them off
                this.EffectAlgorithm = (EffectAlgorithm)(effectAlgorithm & 0x03);

                int status = 0;  // holds the status of MemoryStream.ReadByte

                byte[] reverbData = new byte[ReverbSettings.DataSize];
                status = ms.Read(reverbData);
                this.Reverb = new ReverbSettings(reverbData, 0);

                byte[] effect1Data = new byte[EffectSettings.DataSize];
                status = ms.Read(effect1Data);
                this.Effect1 = new EffectSettings(effect1Data, 0);

                byte[] effect2Data = new byte[EffectSettings.DataSize];
                status = ms.Read(effect2Data);
                this.Effect2 = new EffectSettings(effect2Data, 0);

                byte[] effect3Data = new byte[EffectSettings.DataSize];
                status = ms.Read(effect3Data);
                this.Effect3 = new EffectSettings(effect3Data, 0);

                byte[] effect4Data = new byte[EffectSettings.DataSize];
                status = ms.Read(effect4Data);
                this.Effect4 = new EffectSettings(effect4Data, 0);

                byte[] geqData = new byte[GEQSettings.DataSize];
                status = ms.Read(geqData);
                this.GEQ = new GEQSettings(geqData, 0);

                byte[] nameData = new byte[PatchName.Length];
                this.Name = new PatchName(nameData, 0);

                this.Volume = new PositiveLevel((byte) ms.ReadByte());

                // TODO: Parse the mute byte
                var muteByte = (byte) ms.ReadByte();

                byte[] ec1Data = new byte[3];
                status = ms.Read(ec1Data);
                this.EffectControl1 = new EffectControl(ec1Data);

                byte[] ec2Data =new byte[3];
                status = ms.Read(ec2Data);
                this.EffectControl2 = new EffectControl(ec2Data);
            }
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add((byte)this.EffectAlgorithm);
                data.AddRange(this.Reverb.Data);
                data.AddRange(this.Effect1.Data);
                data.AddRange(this.Effect2.Data);
                data.AddRange(this.Effect3.Data);
                data.AddRange(this.Effect4.Data);
                data.AddRange(this.GEQ.Data);
                data.AddRange(this.Name.Data);
                data.Add(this.Volume.ToByte());

                // TODO: the section mutes byte
                data.Add(0);

                data.AddRange(this.EffectControl1.Data);
                data.AddRange(this.EffectControl2.Data);

                return data;
            }
        }

        public int DataLength => DataSize;
    }
}
