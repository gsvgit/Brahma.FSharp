open TTA.VSFG

(*
let rec f x y =
    if x < y 
    then f x + 1 y
    else x + y
*)

// x < y

let _x1 = new InPort()
let _y1 = new InPort()
let _x_y1 = new OutPort()
let x_l_y = new BinOpBlock(_x1,_y1,BinOp.Less,_x_y1)

//x+y

let _x2 = new InPort()
let _y2 = new InPort()
let _x_y2 = new OutPort()
let x_a_y = new BinOpBlock(_x2,_y2,BinOp.Sum,_x_y2)

// x + 1

let _x3 = new InPort()
let _x_1 = new OutPort()
let x_incr = new UnOpBlock(_x3,UnOp.AddC 1,_x_1)

// if

let pred = new InPort(_x_y1)
let _true = new InPort()
let _false = new InPort(_x_y2)
let if_out = new OutPort()
let _if = new Multiplexor(pred,_true,_false,if_out)

//f

let dummy_x_out = new OutPort()
let dummy_y_out = new OutPort()

let x_f = new InPort(dummy_x_out)
let y_f = new InPort(dummy_y_out)
let f_out = if_out

dummy_x_out.ConnectWith _x3
dummy_x_out.ConnectWith _x2
dummy_x_out.ConnectWith _x1

dummy_y_out.ConnectWith _y2
dummy_y_out.ConnectWith _y1





