var dbdy=document.body;
var cutn=vidarr.length;
var vidcloz=new Array(5);
dbdy.style.backgroundColor='#400'

function videle(n)
{
	var vio= document.createElement('video');
	vio.muted=true;
	vio.autoplay=true;
	vio.controls=true;
	vio.loop=true;
	
	vidarr[n][0]=vio;
	vio.style.display='none';
	return vio;
}
var tele=null;

function undel()
{
	tele=this.parentElement;
	tele.style.backgroundColor='transparent'

	var i=this.ina;
	this.title='>   DEL'+i+'   ( '+vidarr[i][2]+'k )<';
	this.onclick=rziz;
	tele=this;
	
	vidarr[i][0].src='/bak/'+i;
	vidarr[i][1]=false;
	
	
	setTimeout(function(){vidarr[i][0].src=sigshort+'/'+i+'.mp4';}, 500);

	return true;
}


function rziz()
{
	tele=this.parentElement;
	tele.style.backgroundColor='#777'
	var i=this.ina;
	this.title='>   UNDEL'+i+'   ( '+vidarr[i][2]+'k )<';
	this.onclick=undel;
	console.log('mvmp4("/content/'+sigshort+'/rmv/'+i+'.mp4")');
	
	tele=this;

	
	vidarr[i][0].src='/del/'+i;
	vidarr[i][1]=true;


	return true;
}

function vid800()
{
    this.onplaying=null;
    if(this.videoWidth<400)
    {
	for(var i=0;i<cutn;i++)
	{
		vidarr[i][0].width=400;
	}
        
    }
}
var curshow=5;

function mkblkz()
{
	for(var i=0;i<cutn;i++)
	{
		
		var blo= document.createElement('p');
		//blo.style.verticalalign= 'middle';
		var lnk= document.createElement('img');
		lnk.src=sigshort+'/'+i+'.mp4.jpg'
		lnk.style.display ='inline';
		lnk.ina=i;
		lnk.title='>   DEL'+i+'   ( '+vidarr[i][2]+'k )<';
		lnk.onclick=rziz;
		blo.appendChild(lnk);
		blo.appendChild(videle(i));
		
		dbdy.appendChild(blo);
		
	}
	for(var i=0;i<5;i++)
	{
		var vio=vidarr[i][0];
		vidcloz[i]=vio;
		vio.src=sigshort+'/'+i+'.mp4';
		vio.style.display ='inline';
		vio.playbackRate=0.3;
	}
	
	vidarr[0][0].onplaying=vid800;

}

function hydshow()
{
	
	
	for(var i=0;i<5;i++)
	{
		vidcloz[i].src='';
		vidcloz[i].style.display='none';
	}

	var k=0;
	var negend=curshow+5;
	if(negend>cutn){negend=cutn; curshow=cutn-5;}
	for(;curshow<negend;curshow++)
	{
		var vio=vidarr[curshow][0];
		vidcloz[k]=vio;
		vio.src=sigshort+'/'+curshow+'.mp4';
		vio.style.display='inline';
		vio.playbackRate=0.3;
		k++;
	}

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
	var ele=e.target;

if(ele.tagName=='IMG')
{
	curshow=ele.ina;
	hydshow();
	return false;
}
	return true;
}

document.onkeydown=kycmd;
document.oncontextmenu=kontimg;

mkblkz();
