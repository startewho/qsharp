import { add } from "./math.js"
import { printf } from "std";
import { world,Add,Sub } from "hello";


let c = add(1, 2);

printf("std moudle printf\n");

printf("js moudle \n");
console.log(`math module a+b:${c}`);


printf("custom moudle \n");

console.log("hello module world:", world);

let hAdd = Add(2, 3);
console.log("hello module Add:", hAdd);

hAdd = Sub(2, 3);
console.log("hello module Sub:", hAdd);