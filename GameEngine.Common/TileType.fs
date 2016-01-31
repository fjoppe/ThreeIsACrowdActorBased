namespace GameEngine.Common

open WebSharper

[<NamedUnionCases>]
type TileType = 
    | none = 0
    | board = 1
    | blue = 2
    | red = 3
    | yellow = 4
