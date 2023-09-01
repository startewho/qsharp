import { add } from "./math.js"
import { printf } from "std";
import { world,Add1 } from "hello";

let c = add(1, 2);
printf("c");
console.log("c:", c);
printf("c");


console.log("world:", world);

let hAdd = Add1(2, 3);

console.log("hAdd:", hAdd);