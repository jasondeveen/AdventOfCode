use std::{env::args, fs, time::SystemTime};

static PART2: bool = true;

fn main() {
    let now = SystemTime::now();
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

    println!("{:?}", now.elapsed().unwrap());
    println!("Total: {total}");
}

fn calculate(operands: &[&str], operator: &str) -> u64 {
    let operation: fn(u64, u64) -> u64;
    operation = match operator {
        "*" => |a, b| a * b,
        "+" => |a, b| a + b,
        _ => panic!("Unknown operator! {operator}"),
    };

    if PART2 {
        let wacky_operands = get_wacky_operands(operands);
        let wacky_operands: Vec<&str> = wacky_operands.iter().map(|s| s.as_str()).collect();

        return apply_operation(&wacky_operands, operation);
    } else {
        return apply_operation(operands, operation);
    }
}

fn apply_operation(operands: &[&str], operation: fn(u64, u64) -> u64) -> u64 {
    return operands
        .iter()
        .map(|s| s.trim().parse::<u64>().expect("Failed to parse number!"))
        .reduce(|acc, e| operation(acc, e))
        .expect("Operation failed!");
}

fn get_wacky_operands(operands: &[&str]) -> Vec<String> {
    let mut longest_operand_length = 0;
    for operand in operands {
        if operand.len() > longest_operand_length {
            longest_operand_length = operand.len();
        }
    }
    let mut padded_operands = Vec::new();
    for o in operands {
        padded_operands.push(format!("{:>width$}", o, width = longest_operand_length));
    }

    let mut wacky_values = vec![String::from(""); longest_operand_length];
    for i in (0..longest_operand_length).rev() {
        for operand in &padded_operands {
            wacky_values[i].push(
                operand
                    .chars()
                    .nth(i)
                    .expect("expected number or whitespace"),
            );
        }
    }
    wacky_values
}

fn parse_input<'a>(raw_input: &'a String) -> Vec<impl Iterator<Item = &'a str>> {
    let mut iterators = Vec::new();

    for line in raw_input.lines() {
        iterators.push(line.split_whitespace());
        // PROBLEEM: het splitten op whitespace verliest de info van hoe de cijfers onder elkaar
        // staan. we zouden dus moeten de lijnen splitten zodat elke kolom even breed is, en dan
        // moeten we later niet meer kijken voor te padden
    }

    iterators
}
