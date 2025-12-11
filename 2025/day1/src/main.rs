use std::env;
use std::fs;

fn main() {
    let args: Vec<String> = env::args().collect();

    let contents_raw: String =
        fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let contents = contents_raw.split_whitespace();

    let mut current = 50;
    let mut zero_hits = 0;

    for line in contents.into_iter() {
        let temp = current + translate_move(line);
        current = if temp < 0 {
            100 - temp.abs()
        } else if temp >= 100 {
            temp - 100
        } else {
            temp
        };

        current %= 100;

        if current == 0 {
            zero_hits += 1;
        }
    }

    println!("Zero hits: {zero_hits}")
}

fn translate_move(a_move: &str) -> i32 {
    let distance = a_move[1..]
        .trim()
        .parse::<i32>()
        .expect("String to parse was not an integer!");
    let mut direction = 1;

    if a_move.contains('L') {
        direction = -1;
    }

    direction * distance
}
