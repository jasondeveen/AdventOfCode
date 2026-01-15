use std::{fs, time::SystemTime};

fn main() {
    let args: Vec<String> = std::env::args().collect();
    let raw_input: Vec<String> = fs::read_to_string(&args[1])
        .expect("Couldnt find inputfile")
        .lines()
        .map(|str| str.to_owned())
        .collect();

    let now = SystemTime::now();

    println!("elapsed: {:?}", now.elapsed().unwrap());
}
