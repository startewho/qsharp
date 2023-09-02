use std::error::Error;

fn main() -> Result<(), Box<dyn Error>> {
    bindgen::Builder::default()
        .header("quickjspp/quickjs-libc.h")
        .header("quickjspp/quickjs.h")
        
        .generate()
        .unwrap()
        .write_to_file("src/quickjs.rs")
        .unwrap();

    // using cc, build and link c code
    cc::Build::new()
        .files([
            "quickjspp/cutils.c",
            "quickjspp/libbf.c",
            "quickjspp/libregexp.c",
            "quickjspp/libunicode.c",
            "quickjspp/quickjs-libc.c",
            "quickjspp/quickjs-libc.c",
            "quickjspp/quickjs.c",
        ])
        .define("CONFIG_BIGNUM",None)
        .define("CONFIG_DEBUGGER", None)
        .compile("quickjs");

    // csbindgen code, generate both rust ffi and C# dll import
    csbindgen::Builder::default()
        .input_bindgen_file("src/quickjs.rs") // read from bindgen generated code
        .rust_file_header("use super::quickjs::*;") // import bindgen generated modules(struct/method)
        .csharp_dll_name("quickjs")
        .csharp_class_accessibility("public")
        .csharp_namespace("QuickJs") 
        .csharp_entry_point_prefix("csbindgen_")
        .generate_to_file("src/quickjs_ffi.rs", "../Native/NativeMethods.quickjs.g.cs")
        .unwrap();
    Ok(())
}
