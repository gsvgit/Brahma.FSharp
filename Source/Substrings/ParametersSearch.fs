module ParametersSearch

let Main () =
    for i in 1..10 do
        ConsoleLauncher.launch (200*i) 512 InputGenerator.path TemplatesGenerator.path

    ignore (System.Console.Read())

//do InputGenerator.Main()