module arr

open TTA.ASM
open main
 
let func a b d e f g h =  
  
      let arr2: Program = [|[|Set((0<ln>,2<col>),b); Mvc((0<ln>,2<col>),d); Mov((0<ln>,0<col>),(0<ln>,2<col>)); 
                                Set((1<ln>,2<col>),e); Mvc((1<ln>,2<col>),f); Mov((0<ln>,0<col>),(1<ln>,2<col>)); Set((0<ln>,3<col>),g); 
                                 Mov((0<ln>,3<col>),(0<ln>,0<col>)); Set((0<ln>,1<col>),a); Mov((0<ln>,1<col>),(0<ln>,3<col>)); Mvc((0<ln>,1<col>),h)|]|]
      
      TTA.Translator.asmToIL arr2 |> main.run |> ignore
      main.getVal 0 1 
        
      // a - g / (b * d + e * f) - h
      // 10 - 20 / (2 * 3 + 4 * 1)  - 7
    
func 10 2 3 4 1 20 7  |> printfn "%A"

//func 1 3 4 -3 1 45 10 |> printfn "%A"   