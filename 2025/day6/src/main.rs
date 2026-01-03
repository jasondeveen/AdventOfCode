use std::{env::args, fs, time::SystemTime};

static PART2: bool = true;

#[derive(Debug)]
struct Input<'a> {
    lines: Vec<&'a str>,
}

impl<'a> Input<'a> {
    fn next(&mut self) -> Option<Vec<String>> {
        let mut result = Vec::new();

        let word_length = self
            .lines
            .last()
            .expect("No lines found")
            .chars()
            .skip(1)
            .take_while(|c| c.is_whitespace())
            .count();

        if word_length == 0 {
            return None;
        }

        let EOL_reached = if self.lines.last().unwrap().len() == word_length + 1 {
            1
        } else {
            0
        };

        for line in &mut self.lines {
            result.push(
                line.chars()
                    .take(word_length + EOL_reached)
                    .collect::<String>(),
            );
            *line = &line[word_length + 1..]; // skip over empty line column
        }

        Some(result)
    }
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = args().collect();
    let raw_input = fs::read_to_string(&args[1]).expect("Input not found! {&args[1]}");

    let mut input = Input {
        lines: raw_input.lines().collect(),
    };

    let mut total = 0;
    loop {
        let current_values;
        let temp = input.next();
        match temp {
            Some(val) => current_values = val,
            None => break,
        }

        total += calculate(
            &current_values[..current_values.len() - 1],
            &current_values[current_values.len() - 1],
        );
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("Total: {total}");
}

fn calculate(operands: &[String], operator: &String) -> u64 {
    let operation: fn(u64, u64) -> u64;
    operation = match operator.trim() {
        "*" => |a, b| a * b,
        "+" => |a, b| a + b,
        _ => panic!("Unknown operator! {operator}"),
    };

    if PART2 {
        let wacky_operands = get_wacky_operands(operands);
        let wacky_operands = wacky_operands;

        return apply_operation(&wacky_operands, operation);
    } else {
        return apply_operation(operands, operation);
    }
}

fn apply_operation(operands: &[String], operation: fn(u64, u64) -> u64) -> u64 {
    return operands
        .iter()
        .map(|s| s.trim().parse::<u64>().expect("Failed to parse number!"))
        .reduce(|acc, e| operation(acc, e))
        .expect("Operation failed!");
}

fn get_wacky_operands(operands: &[String]) -> Vec<String> {
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

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_input() {
        let test_cases = vec![(
            Input {
                lines: vec![
                    " 123 328  51 64  ",
                    "  45 64  387 23  ",
                    "   6 98  215 314 ",
                    " *   +   *   +   ",
                ],
            },
            [
                ["123", " 45", "  6", "*  "],
                ["328", "64 ", "98 ", "+  "],
                [" 51", "387", "215", "*  "],
                ["64 ", "23 ", "314", "+  "],
            ],
        )];

        for (mut input, expected_results) in test_cases {
            for expected_result in expected_results {
                let result;
                let temp = input.next();
                match temp {
                    Some(val) => result = val,
                    None => break,
                }

                assert_eq!(result.len(), expected_result.len());
                for i in 0..expected_result.len() {
                    assert_eq!(expected_result[i], result[i]);
                }
            }
        }
    }
}
