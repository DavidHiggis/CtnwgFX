
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Security.Cryptography;
using SevenZipExtractor;

namespace fysvr
{
	public class webserv
	{
		public virtual string rsp(string url, HttpListenerResponse response)
		{
			return "fake";
		}
	}

	/*
	public class md5f : webserv
	{
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			return "msg";
		}
		const string key="/~md5chk";
		public override string ToString()
		{
			return key;
		}
	}
	*/
	
	public class LargeGz : webserv
	{
		public FileStream fs=null;
		public int chunkC=0;
		public int chunkLzt=0;
		public int chunki=0;
		IncrementalHash iha;
		string lpa;
		
		//		http://127.0.0.1:7414/~lseggz/avifenc.exe
		//		http://127.0.0.1:7414/~md5chk/avifenc.exe
		public void doGz(HttpListenerResponse response,int size)
		{
			response.Headers.Add("Content-Encoding", "gzip");
			using (var zipStream = new GZipStream(response.OutputStream, CompressionMode.Compress)) {
				byte[] buf = new byte[size];
				fs.Read(buf,0,size);
				iha.AppendData(buf);
				zipStream.Write(buf, 0, size);
				zipStream.Close();
			}
		}
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			
			if(url[8]=='/')
			{
				if(fs!=null)
				{
					fs.Close();
					fs=null;
				}
				iha=  IncrementalHash.CreateHash(HashAlgorithmName.MD5);
				lpa=url.Substring(8);
				fs=File.OpenRead(serv.Home+lpa);
				long flen=fs.Length;
				chunkC=(int)(flen>>22);
				chunkLzt=(int)(flen&0x3FFFFF);
				chunki=0;
				
				
				string lptime=((chunkLzt==0)?chunkC:chunkC+1).ToString();
				serv.StrRsp(lptime,response);
				return lptime;
			}
			if(fs==null){
				serv.ret404(response);
				return "NoFs";
			
			}
			string msg=string.Empty;
			if(url.Length>9)
			{
				long seeko=long.Parse(url.Substring(9))<<22;
				if(fs.Position!=seeko)
				{
					fs.Position=seeko;
					msg+="\nseekto "+seeko.ToString("X");
				}
				
			}
			if(chunki<chunkC)
			{
				doGz(response,0x400000);
				chunki++;
				return msg+"\nGet chunk"+(chunki-1);
			}
			else
			{
				if(chunkLzt!=0)
				{
					doGz(response,chunkLzt);
				}
				fs.Close();
				fs=null;
				md5f.cache[md5f.key+lpa]=iha.GetHashAndReset();
				return msg+"\nfin";
			}
			
			
			return msg;
		}
		const string key="/~lseggz";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class InvFile : webserv
	{
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			string lpa=url.Substring(8);
			byte[] buf = File.ReadAllBytes(serv.Home+lpa);
			int bufl=buf.Length;
			md5f.cache[md5f.key+lpa]=md5f.calc.ComputeHash(buf);
			
			for(int i=0;i<bufl;i++)
			{
				buf[i]^=0xff;
			}
			
			
			response.OutputStream.Write(buf, 0, bufl);
			
			return "Inv "+lpa;
		}
		const string key="/~invfsm";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class InvLargeFile : webserv
	{
		//		http://127.0.0.1:7414/~invfbg/kO-ix0ptViHBDDRn.mpp
		public override string rsp(string url, HttpListenerResponse response)
		{
			string lpa=url.Substring(8);
			
			using(FileStream fs=File.OpenRead(serv.Home+lpa))
			{
				IncrementalHash iha=  IncrementalHash.CreateHash(HashAlgorithmName.MD5);
				
				byte[] tmpbuf=new byte[0x400000];
				long flen=fs.Length;
				int blkcot=(int)(flen>>22);
				
				response.ContentLength64 = flen;
				
				for(int i=0;i<blkcot;i++)
				{
					fs.Read(tmpbuf,0,0x400000);
					iha.AppendData(tmpbuf);
					
						for(int k=0;k<0x400000;k++)
						{
							tmpbuf[k]^=0xff;
						}
					
					response.OutputStream.Write(tmpbuf,0,0x400000);
					response.OutputStream.Flush();
					
					
				}
				int rzt=(int)(flen-(((long)blkcot)<<22));
				if(rzt>0)
				{
					fs.Read(tmpbuf,0,rzt);
					iha.AppendData(tmpbuf,0,rzt);
					for(int i=0;i<rzt;i++)
					{
							tmpbuf[i]^=0xff;
					}
					
					response.OutputStream.Write(tmpbuf,0,rzt);
					response.OutputStream.Flush();
					
				}
				md5f.cache[md5f.key+lpa]=iha.GetHashAndReset();
				
				
			}
			return "InvLarge: "+lpa;
		}
		const string key="/~invfbg";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class gzFile : webserv
	{
		//		http://127.0.0.1:7414/~compgz/daly.htm
		public override string rsp(string url, HttpListenerResponse response)
		{
			string lpa=url.Substring(8);
			using (var zipStream = new GZipStream(response.OutputStream, CompressionMode.Compress)) {
				byte[] buf = File.ReadAllBytes(serv.Home+lpa);
				int bufl=buf.Length;
				zipStream.Write(buf, 0, buf.Length);
				zipStream.Close();
			}
			return "gz "+lpa;
		}
		const string key="/~compgz";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class hugeFile : webserv
	{
		//		http://127.0.0.1:7414/~hugeff/avifenc.exe
		//		http://127.0.0.1:7414/~md5chk/avifenc.exe
		public override string rsp(string url, HttpListenerResponse response)
		{
			
			string lpa=url.Substring(8);
			//using(BufferedStream bstm = new BufferedStream(File.OpenRead(pa), 0x100000))
			using(FileStream fs=File.OpenRead(serv.Home+lpa))
			{
				IncrementalHash iha=  IncrementalHash.CreateHash(HashAlgorithmName.MD5);
				
				byte[] tmpbuf=new byte[0x400000];
				long flen=fs.Length;
				int blkcot=(int)(flen>>22);
				
				response.ContentLength64 = flen;
				
				for(int i=0;i<blkcot;i++)
				{
					fs.Read(tmpbuf,0,0x400000);
					response.OutputStream.Write(tmpbuf,0,0x400000);
					response.OutputStream.Flush();
					iha.AppendData(tmpbuf);
					
				}
				int rzt=(int)(flen&0x3FFFFF);
				if(rzt>0)
				{
					fs.Read(tmpbuf,0,rzt);
					response.OutputStream.Write(tmpbuf,0,rzt);
					response.OutputStream.Flush();
					iha.AppendData(tmpbuf,0,rzt);
				}
				md5f.cache[md5f.key+lpa]=iha.GetHashAndReset();
				
				
			}
			return "large: "+lpa;
		}
		const string key="/~hugeff";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class search: webserv
	{
		//	http://127.0.0.1:7414/~search/~/a3/kmhm~*.avif~
		
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			string[] zs=url.Split(utily.sepTilda);
			
			
			string[] flist=Directory.GetFiles(serv.Home+zs[2],zs[3],(zs.Length>4) ? SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
			int fl=flist.Length;
			for(int i=0;i<fl;i++)
			{
				flist[i]=flist[i].Substring(serv.Home.Length).Replace("\\","/");
			}
			serv.StrRsp(string.Join("\n",flist) ,response);
			return "Search "+zs[3]+" in "+zs[2];
		}
		const string key="/~search";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class zearch: webserv
	{
		//	http://127.0.0.1:7414/~zearch/0f/xtract.zip
		
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			string archivpa=url.Substring(8);
			ArchiveFile az7;
			if(!zipped.cache.TryGetValue(archivpa,out az7))
			{
				string fpa=serv.Home+archivpa;
				if(File.Exists(fpa))
				{
					az7=new ArchiveFile(fpa);
					zipped.cache[archivpa]=az7;
				}
				else
				{
					serv.ret404(response);
					return "NoZip: "+fpa;
				}
			}
			
			int fl=0;
			string[] flist=new string[az7.Entries.Count];
			
			foreach(Entry enty in az7.Entries)
			{
				flist[fl]="/~zipsys/~"+archivpa+"~"+enty.FileName.Replace("\\","/");
				fl++;
			}
			
			serv.StrRsp(string.Join("\n",flist) ,response);
			return "listZip";
		}
		const string key="/~zearch";
		public override string ToString()
		{
			return key;
		}
	}
	
	public class zipped : webserv
	{
		public static Dictionary<string,ArchiveFile> cache=new Dictionary<string,ArchiveFile>();
		//	http://127.0.0.1:7414/~zipsys/~/0f/xtract.zip~lib/net45/SevenZipExtractor.dll
		
		//	http://127.0.0.1:7414/~md5chk/0f/xtract.zip~lib/net45/SevenZipExtractor.dll
		
		
		
		
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			string[] zs=url.Split(utily.sepTilda);
			string archivpa=zs[2];
			
			
				
			ArchiveFile az7;
			if(!cache.TryGetValue(archivpa,out az7))
			{
				string fpa=serv.Home+archivpa;
				if(File.Exists(fpa))
				{
					az7=new ArchiveFile(fpa);
					cache[archivpa]=az7;
				}
				else
				{
					serv.ret404(response);
					return "NoZip: "+fpa;
				}
			}
			
			string finz=zs[3].Replace("/","\\");
			foreach(Entry enty in az7.Entries)
			{
				if(enty.FileName.ToLower()==finz)
				{
					enty.Extract(response.OutputStream);
					md5f.cache[md5f.key+url.Substring(10)]=utily.uint2strbyte(enty.CRC);
					return "Get "+finz+" in "+archivpa;
				}
			}
				
			serv.ret404(response);
			return "No "+finz+" in "+archivpa;
					
					
			
			
			
		}
		const string key="/~zipsys";
		public override string ToString()
		{
			return key;
		}
	}

	public class cleaner : webserv
	{
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			string zs = serv.Nthcmd(url);
			switch(zs)
			{
				case "md5":
					md5f.cache=new Dictionary<string,byte[]>();
					break;
				case "7z":
					foreach(var kv in zipped.cache)
					{
						kv.Value.Dispose();
					}
					zipped.cache=new Dictionary<string,ArchiveFile>();
					
					break;
				default:
					break;
			}
			string msg="Clear "+zs;
			serv.StrRsp(msg,response);
			return msg;
			
		}
		const string key="/~cleanr";
		public override string ToString()
		{
			return key;
		}
	}
	
	
	public class md5f : webserv
	{
		public static MD5 calc = MD5.Create();
		public static Dictionary<string,byte[]> cache=new Dictionary<string,byte[]>();
		
		
		string showall(HttpListenerResponse response)
		{
			
			foreach(var kv in cache)
			{
				byte[] buf=Encoding.ASCII.GetBytes("\n"+kv.Key.Substring(8)+"\t");
				response.OutputStream.Write(buf,0,buf.Length);
				response.OutputStream.Write(kv.Value,0,kv.Value.Length);
				
			}
			
			return "allmd5";
		}
		
		public override string rsp(string url, HttpListenerResponse response)
		{
			if(url.Length<10)
			{
				return showall(response);
			}
			
			byte[] buf=cache[url];
			int bufl=buf.Length;
			//response.ContentLength64 = bufl;
			response.OutputStream.Write(buf, 0, bufl);
			
			return url;
		}
		public const string key="/~md5chk";
		public override string ToString()
		{
			return key;
		}
	}

	public static class serv
	{
		public const string port= "7414";
		public const string Home = @"Q:\z\foxgi\gif";
		public static Dictionary<string,webserv> pipe=new Dictionary<string, webserv>();
		
		
		public static string[] cmdcache;
		public static string Nthcmd(string url,int nth=0)
		{
			cmdcache=url.Split(utily.sepLpa);
			return cmdcache[2+nth];
		}
		public static string Nthcmd(int nth=0)
		{
			return cmdcache[2+nth];
		}
		
		public static void ret404(HttpListenerResponse response)
		{
			//response.ContentLength64 = 0x10;
			response.StatusCode=404;
			response.OutputStream.Write(serv.str404, 0, 0x10);
		}
		
		public static void StrRsp(string msg,HttpListenerResponse response)
		{
			byte[] buf=Encoding.ASCII.GetBytes(msg);
			int bufl=buf.Length;
			//response.ContentLength64 = bufl;
			response.OutputStream.Write(buf, 0, bufl);
		}
		
		public static void setpipe(webserv svr)
		{
			pipe[svr.ToString()]=svr;
		}
		
		public static byte[] str404= {0x21, 0x21, 0x34, 0x30, 0x34, 0x20, 0x4E, 0x6F, 0x74, 0x20, 0x46, 0x6F, 0x75, 0x6E, 0x64, 0x21};
		
		
		//		http://127.0.0.1:7414/a3/kmhm/1_14.avif
		//		http://127.0.0.1:7414/~md5chk/a3/kmhm/1_14.avif
		public static void start()
		{
			setpipe(new md5f());
			setpipe(new cleaner());
			setpipe(new zipped());
			setpipe(new search());
			setpipe(new zearch());
			setpipe(new hugeFile());
			setpipe(new gzFile());
			setpipe(new InvFile());
			setpipe(new InvLargeFile());
			setpipe(new LargeGz());

			HttpListener server = new HttpListener();
			server.Prefixes.Add("http://127.0.0.1:"+port+"/");
			server.Prefixes.Add("http://localhost:"+port+"/");
			server.Start();
			Console.WriteLine("Listening...");
			while (server.IsListening) {
					HttpListenerContext context = server.GetContext();
					HttpListenerResponse response = context.Response;
	                
					response.StatusCode = 200;
					
	                
					string lpa = context.Request.Url.LocalPath.ToLower();

					string msg=string.Empty;
					if(lpa[1]=='~') { msg=pipe[lpa.Substring(0,8)].rsp(lpa, response); }
					else
					{
						msg=Home+lpa;
						byte[] buf=null;
						int bufl=0;
						if(File.Exists(msg))
						{
							buf = File.ReadAllBytes(msg);
							bufl=buf.Length;
							md5f.cache[md5f.key+lpa]=md5f.calc.ComputeHash(buf);
						}
						else
						{
							context.Response.StatusCode = 404;
							buf=str404;
							bufl=0x10;
						}
						response.ContentLength64 = bufl;
						response.OutputStream.Write(buf, 0, bufl);
					}
					
					
					
					
					
					context.Response.Close();
					Console.WriteLine(msg);
	                
				}
			}
		
		
		/*
		public static Dictionary<string,webserv> pipe
		{
			get{return rpipe;}
			set{}
		}
		*/
		
	}

	
	static class utily
	{
	
		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		public static extern int system(string command);
		public static char[] sepWpa = { '\\' };
		public static char[] sepLpa = { '/' };
		public static char[] sepTilda = { '~' };
		
		public static byte strbyte(byte x)
		{
			if(x>9)
			{
				return (byte)(0x57+x);
			}
			return (byte)(0x30+x);
		}
		
		public static byte[] mdx2strbyte(byte[] x)
		{
			
			byte[] ret = new byte[32];
			int zf=0;
			for(int j=0;j<16;j++)
			{
				ret[zf]=strbyte((byte)((x[j]>>4)&0xF));
				zf++;
				ret[zf]=strbyte((byte)(x[j]&0xF));
				zf++;
			}
			return ret;
			
		}
		
		public static byte[] uint2strbyte(uint x)
		{
			byte[] ret = new byte[8];
			
			uint mv=x;
			int zf=7;
			for(int j=0;j<4;j++)
			{
				
				uint mskd=mv&0xF;
				ret[zf]=strbyte((byte)mskd);
				mskd=(mv>>4)&0xF;
				zf--;
				ret[zf]=strbyte((byte)mskd);
				zf--;
				mv>>=8;
			
			}
			return ret;
		}
		
		
		static void Main(string[] args)
		{
			system("start \"\" ngrok.exe http "+serv.port+" -host-header=\"localhost:"+serv.port+"\"");
			serv.start();
			
		}
		
		/*
		static void tzt7z()
		{
					using (ArchiveFile archiveFile = new ArchiveFile(@"Q:\z\foxgi\gif\sevenzipextract.zip"))
					{
					    foreach (Entry entry in archiveFile.Entries)
					    {
					        Console.WriteLine(entry.FileName);
					        
					       
					    }
					}
		}
		*/
		
	}
}
