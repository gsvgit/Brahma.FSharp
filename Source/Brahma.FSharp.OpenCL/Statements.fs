// Copyright (c) 2012 Semyon Grigorev <rsdpisuy@gmail.com>
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

namespace Brahma.FSharp.OpenCL.AST

type VarDecl<'lang> (vType:Type<'lang>,name:string ,expr:Option<Expression<'lang>>) =
    inherit Statement<'lang>()
    override this.Children = []
    member this.Type = vType
    member this.Name = name
    member this.Expr = expr

type Assignment<'lang> (vName:Property<'lang>,value:Expression<'lang>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Name = vName
    member this.Value = value

type StatementBlock<'lang> (statements:ResizeArray<Statement<'lang>>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Statements = statements    

type IfThenElse<'lang> (cond:Expression<'lang>, thenBranch:StatementBlock<'lang>, elseBranch:Option<StatementBlock<'lang>>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Condition = cond
    member this.Then = thenBranch
    member this.Else = elseBranch

type ForIntegerLoop<'lang> (var:VarDecl<'lang>, cond:Expression<'lang>, countModifier:Expression<'lang>, body:StatementBlock<'lang>)=
    inherit Statement<'lang>()
    override this.Children = []
    member this.Var = var
    member this.Condition = cond
    member this.CountModifier = countModifier
    member this.Body = body