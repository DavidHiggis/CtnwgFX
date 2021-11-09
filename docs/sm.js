

curshow=0;


var dbdy=document.body;
dbdy.style.backgroundColor='#400';
var imgar=new Array(5);
var duzl=duuz.length;
var blockar=new Array(duzl);
function setup()
{
	curshow=0;
	dbdy.innerHTML='';
	dbdy.style.color='#fff';
	dbdy.style.width="100%";
	for(var i=0;i<duzl;i++)
	{
		
		var lnk= document.createElement('img');
		lnk.ina=i;
		lnk.title=duuz[i]+'.';
		
		
		blockar[i]=lnk;
		dbdy.appendChild(lnk);
	}

	doshow();
}
	
function exchange(src,dst,n)
	{
		for(var i=0;i<n;i++)
		{
			var tmp=duuz[dst+i];
			duuz[dst+i]=duuz[src+i];
			duuz[src+i]=tmp;
		}
	}
	
function shuf()
	{
		var k=1;
		var endo=duzl>>1;
		for(var i=duzl-1;i>endo;i--)
		{
			exchange(i,(i*Math.random())>>0,k&0x3f);
			k++;
		}
		setup();
	}
	
function segsep(tg,lit)
	{
		var viu=document.getElementsByTagName(tg);
		var viul=viu.length;
		for(var i=0;i<viul;i++)
		{
			var chl=viu[i].children.length;
			if(chl>lit)
			{
				
				for(var jj=lit;jj<chl;jj++)
				{
					viu[i].insertBefore(document.createElement('br'),viu[i].children[jj]);
				}
			}
		}
		
	}
function setup_simi()
{
	curshow=0;
	dbdy.innerHTML='';
	dbdy.style.color='#000';
	dbdy.style.width="300%";
	duzl=simi.length/3;
	duuz=new Array(duzl);
	blockar=new Array(duzl);
	
	var lvl=new Array(10);
	for(var i=0;i<duzl;i++)
	{
		duuz[i]=simi[i*3+1];
		var blo= null;
		var blvl=simi[i*3];
		var lnk= document.createElement('img');
		lnk.ina=i;
		lnk.title=duuz[i]+'.';
		blockar[i]=lnk;
		
		if(blvl==0)
		{
			blo= document.createElement('p');
			blo.innerHTML='<h1>base: '+duuz[i]+'</h1>';
			lvl[0]=blo;
			blo.appendChild(lnk);
			blo.appendChild(document.createElement('br'));
			dbdy.appendChild(blo);
		}
		else
		{
			blo= document.createElement('V');
			blo.innerHTML=simi[i*3+2]+'%=======';
			lvl[blvl]=blo;
			blo.appendChild(lnk);
			lvl[blvl-1].appendChild(blo);
		}
		var caa=0xf0-(blvl*0x20);
		blo.style.backgroundColor='rgb(255,'+caa+',255)';
		
		
		
	}
	segsep('p',4);
	segsep('V',2);
	
	
	doshow();

}
	
	
function doshow()
{
var k=0;
var endo=curshow+5;
if(endo>duzl){endo=duzl; curshow=duzl-5;}
	for(;curshow<endo;curshow++)
	{
		imgar[k]=blockar[curshow];
		
		blockar[curshow].src=duuz[curshow]+'.avif';
		
		k++;
	}

}

function diffsig()
{
	var beginz=[];
	var txtoutcole=[];
	for(var i=0;i<duzl;i++)
	{
		if(simi[i*3]==0)
		{
			 beginz.push(i);
		}
	}
	var begl=beginz.length;
	beginz.push(duzl);
	
	for(var i=0;i<begl;i++)
	{
		var sta=beginz[i]
		var endo=beginz[i+1];
		var bssig=simi[sta*3+1].substr(0,5);
		sta+=1;
		var isdiff=false;
		for(var j=sta;j<endo;j++)
		{
			if(simi[j*3+1].substr(0,5)!=bssig){isdiff=true;}
		}

		if(isdiff)
		{
			sta-=1;
			for(var j=sta;j<endo;j++)
			{
			txtoutcole.push(simi[j*3]+',"'+simi[j*3+1]+'",'+simi[j*3+2]+',')
			}
		}

	}

	return txtoutcole.join('\n');
	
}

function hydshow()
{
	for(var i=0;i<5;i++)
	{
		imgar[i].src='';
	}

	doshow();
}

var kycmd=function(e) {
    var ekeyCode=e.keyCode;
    switch (ekeyCode) {
        case 101:
        hydshow();
        break;
    }
}

var kontimg=function(e) {
	curshow=e.target.ina;
	hydshow();
	
}

setup();
document.onkeydown=kycmd;
document.onclick=kontimg;

