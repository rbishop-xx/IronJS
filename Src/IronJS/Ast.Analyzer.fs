﻿module IronJS.Ast.Analyzer

(*Imports*)
open IronJS
open IronJS.Utils
open IronJS.Ast.Types
open IronJS.Ast.Helpers

(*Analyzer Functions*)
let analyzeAssign (left:Node) (right:Node) (scopes:Scopes) =
  match left with
  //Assign to local variable
  | Local(name) -> 
    match right with
    | Local(rightName) -> 
      addUsedWith name rightName scopes
      addUsedWith rightName name scopes

    | Global(_) -> addUsedAs name Types.JsTypes.Dynamic scopes
    | Number(_) -> addUsedAs name Types.JsTypes.Double  scopes
    | String(_) -> addUsedAs name Types.JsTypes.String  scopes

    //Ignore
    | _ -> ()

  //Assign to closure variable means we 
  //need to set closure access to Write
  | Closure(name) -> 
    scopes := List.map (fun scope -> 
      if hasLocal scope name 
        then setAccessWrite scope name
        else scope
      ) !scopes

  //Ignore all else
  | _ -> ()

let analyze (ast:Node) (scopes:Scopes) =
  match ast with
  | Assign(left, right) -> analyzeAssign left right scopes

  //Ignore
  | _ -> ()

  ast