(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,Jatek,Container,Artefact,UI,Next,Var,View1,Client,Doc,List,AttrProxy,View,Var1,CancellationTokenSource,Concurrency,GameInternals,Game,Scene,Visual,T;
 Runtime.Define(Global,{
  WebSharper:{
   Jatek:{
    Artefact:Runtime.Class({},{
     New:function()
     {
      return Runtime.New(this,{});
     }
    }),
    Container:Runtime.Class({
     get_Artefacts:function()
     {
      return this.artefacts;
     },
     get_Height:function()
     {
      return Var.Create(this.height);
     },
     get_Visual:function()
     {
      var arg00,arg10,styleView,mapping,artefactList,list,arg001,ctl;
      arg00=View1.Const(function(w)
      {
       return function(h)
       {
        return function(x)
        {
         return function(y)
         {
          return"position: absolute; width: "+Global.String(w)+"px; height: "+Global.String(h)+"px; background-color: lightgray; left: "+Global.String(x)+"px; top: "+Global.String(y)+"px";
         };
        };
       };
      });
      arg10=this.get_Width().get_View();
      styleView=View1.Apply(View1.Apply(View1.Apply(View1.Apply(arg00,arg10),this.get_Height().get_View()),this.get_X().get_View()),this.get_Y().get_View());
      mapping=function(a)
      {
       return a.get_Visual();
      };
      list=this.get_Artefacts();
      artefactList=Doc.Concat(List.map(mapping,list));
      arg001=function(style)
      {
       return Doc.Element("div",List.ofArray([AttrProxy.Create("style",style)]),List.ofArray([artefactList]));
      };
      ctl=Doc.EmbedView(View.Map(arg001,styleView));
      return ctl;
     },
     get_Width:function()
     {
      return Var.Create(this.width);
     },
     get_X:function()
     {
      return Var.Create(this.x);
     },
     get_Y:function()
     {
      return Var.Create(this.y);
     }
    },{
     New:function(x,y,width,height,artefacts)
     {
      var r;
      r=Runtime.New(this,Artefact.New());
      r.x=x;
      r.y=y;
      r.width=width;
      r.height=height;
      r.artefacts=artefacts;
      return r;
     }
    }),
    ContainerModule:{
     Create:function(x,y,w,h,a)
     {
      return Container.New(x,y,w,h,a);
     }
    },
    Game:Runtime.Class({
     Start:function()
     {
      var matchValue,_,lf,cancellationSource;
      matchValue=Var1.Get(this.get_GameState());
      if(matchValue.$==1)
       {
        matchValue.$0[0];
        matchValue.$0[1];
        _=null;
       }
      else
       {
        lf=matchValue.$0;
        cancellationSource=CancellationTokenSource.New();
        Concurrency.Start(GameInternals.frameRefresh(lf),{
         $:1,
         $0:cancellationSource
        });
        _=Var1.Set(this.get_GameState(),{
         $:1,
         $0:[lf,cancellationSource]
        });
       }
      return _;
     },
     Stop:function()
     {
      var matchValue,_,lf,cs;
      matchValue=Var1.Get(this.get_GameState());
      if(matchValue.$==1)
       {
        lf=matchValue.$0[0];
        cs=matchValue.$0[1];
        cs.Cancel();
        _=Var1.Set(this.get_GameState(),{
         $:0,
         $0:lf
        });
       }
      else
       {
        matchValue.$0;
        _=null;
       }
      return _;
     },
     get_CurrentScene:function()
     {
      return Var.Create(this.startScene);
     },
     get_GameState:function()
     {
      return Var.Create({
       $:0,
       $0:this.gameLoop
      });
     },
     get_Height:function()
     {
      return this.height;
     },
     get_Scenes:function()
     {
      return this.scenes;
     },
     get_Visual:function()
     {
      var _,_1,style,currentScene;
      _=this.get_Width();
      _1=this.get_Height();
      style="position: relative; width: "+Global.String(_)+"px; height: "+Global.String(_1)+"px; background-color: lightgray";
      currentScene=this.get_Scenes().get_Item(Var1.Get(this.get_CurrentScene()));
      return Doc.Element("div",List.ofArray([AttrProxy.Create("style",style)]),List.ofArray([currentScene.get_Visual()]));
     },
     get_Width:function()
     {
      return this.width;
     }
    },{
     New:function(width,height,gameLoop,scenes,startScene)
     {
      var r;
      r=Runtime.New(this,{});
      r.width=width;
      r.height=height;
      r.gameLoop=gameLoop;
      r.scenes=scenes;
      r.startScene=startScene;
      return r;
     }
    }),
    GameInternals:{
     frameRefresh:function(updateFunction)
     {
      return Concurrency.Delay(function()
      {
       return Concurrency.Bind(Concurrency.Sleep(1000),function()
       {
        updateFunction(0);
        return GameInternals.frameRefresh(updateFunction);
       });
      });
     }
    },
    GameModule:{
     Create:function(width,height,gameLoop,scenes,startScene)
     {
      return Game.New(width,height,gameLoop,scenes,startScene);
     }
    },
    Scene:Runtime.Class({
     get_Artefacts:function()
     {
      return this.artefacts;
     },
     get_Visual:function()
     {
      var mapping,artefactList,list,arg20,ctl;
      mapping=function(a)
      {
       return a.get_Visual();
      };
      list=this.get_Artefacts();
      artefactList=Doc.Concat(List.map(mapping,list));
      arg20=List.ofArray([artefactList]);
      ctl=Doc.Element("div",[],arg20);
      return ctl;
     }
    },{
     New:function(artefacts)
     {
      var r;
      r=Runtime.New(this,{});
      r.artefacts=artefacts;
      return r;
     }
    }),
    SceneModule:{
     Create:function(artefacts)
     {
      return Scene.New(artefacts);
     }
    },
    Visual:Runtime.Class({
     get_Height:function()
     {
      return Var.Create(this.height);
     },
     get_SourceUrl:function()
     {
      return this.sourceUrl;
     },
     get_Visual:function()
     {
      var arg00,arg10,styleView,arg001,_this=this,ctl;
      arg00=View1.Const(function(w)
      {
       return function(h)
       {
        return function(x)
        {
         return function(y)
         {
          return"position: absolute; width: "+Global.String(w)+"px; height: "+Global.String(h)+"px; background-color: lightgray; left: "+Global.String(x)+"px; top: "+Global.String(y)+"px";
         };
        };
       };
      });
      arg10=this.get_Width().get_View();
      styleView=View1.Apply(View1.Apply(View1.Apply(View1.Apply(arg00,arg10),this.get_Height().get_View()),this.get_X().get_View()),this.get_Y().get_View());
      arg001=function(style)
      {
       return Doc.Element("div",List.ofArray([AttrProxy.Create("style",style)]),List.ofArray([Doc.Element("img",List.ofArray([AttrProxy.Create("src",_this.get_SourceUrl())]),Runtime.New(T,{
        $:0
       }))]));
      };
      ctl=Doc.EmbedView(View.Map(arg001,styleView));
      return ctl;
     },
     get_Width:function()
     {
      return Var.Create(this.width);
     },
     get_X:function()
     {
      return Var.Create(this.x);
     },
     get_Y:function()
     {
      return Var.Create(this.y);
     }
    },{
     New:function(sourceUrl,x,y,width,height)
     {
      var r;
      r=Runtime.New(this,Artefact.New());
      r.sourceUrl=sourceUrl;
      r.x=x;
      r.y=y;
      r.width=width;
      r.height=height;
      return r;
     }
    }),
    VisualModule:{
     Create:function(source,x,y,w,h)
     {
      return Visual.New(source,x,y,w,h);
     }
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  Jatek=Runtime.Safe(Global.WebSharper.Jatek);
  Container=Runtime.Safe(Jatek.Container);
  Artefact=Runtime.Safe(Jatek.Artefact);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Var=Runtime.Safe(Next.Var);
  View1=Runtime.Safe(Next.View1);
  Client=Runtime.Safe(Next.Client);
  Doc=Runtime.Safe(Client.Doc);
  List=Runtime.Safe(Global.WebSharper.List);
  AttrProxy=Runtime.Safe(Client.AttrProxy);
  View=Runtime.Safe(Next.View);
  Var1=Runtime.Safe(Next.Var1);
  CancellationTokenSource=Runtime.Safe(Global.WebSharper.CancellationTokenSource);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  GameInternals=Runtime.Safe(Jatek.GameInternals);
  Game=Runtime.Safe(Jatek.Game);
  Scene=Runtime.Safe(Jatek.Scene);
  Visual=Runtime.Safe(Jatek.Visual);
  return T=Runtime.Safe(List.T);
 });
 Runtime.OnLoad(function()
 {
  Runtime.Inherit(Container,Artefact);
  Runtime.Inherit(Visual,Artefact);
  return;
 });
}());
