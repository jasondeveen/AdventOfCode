use std::{env::args, fs};

fn main() {
    let args: Vec<String> = args().collect();
    let raw_input = fs::read_to_string(&args[1]).expect("Input not found! {&args[1]}");

    let mut line_iterators = parse_input(&raw_input);
    let number_of_lines = line_iterators.len();

    let mut current_values = vec![""; number_of_lines];

    let mut total = 0;
    'outer: loop {
        let mut i = 0;
        for iterator in &mut line_iterators {
            let temp = iterator.next();
            match temp {
                Some(val) => current_values[i] = val,
                None => break 'outer,
            }

            i += 1;
        }

        total += calculate(
            &current_values[..number_of_lines - 1],
            &current_values[number_of_lines - 1],
        )
    }

    println!("Total: {total}");
}

fn calculate(operands: &[&str], operator: &str) -> u64 {
    let operation: fn(u64, u64) -> u64;
    if operator == "*" {
        operation = |a, b| a * b;
    } else if operator == "+" {
        operation = |a, b| a + b;
    } else {
        panic!("Unknown operator! {operator}");
    }

    return operands
        .iter()
        .map(|s| s.parse::<u64>().expect("Failed to parse number!"))
        .reduce(|acc, e| operation(acc, e))
        .expect("Product failed!");
}

fn parse_input<'a>(raw_input: &'a String) -> Vec<impl Iterator<Item = &'a str>> {
    let mut iterators = Vec::new();

    for line in raw_input.lines() {
        iterators.push(line.split_whitespace());
    }

    iterators
}
