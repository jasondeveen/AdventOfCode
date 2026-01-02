use std::{env::args, fs, time::SystemTime};

static PART2: bool = true;

#[derive(Debug)]
struct Input<'a> {
    lines: Vec<&'a str>,
    current_width: usize,
}

impl<'a> Input<'a> {
    fn build(lines: Vec<&'a str>) -> Result<Input<'a>, String> {
        let mut res = Input {
            lines,
            current_width: 0,
        };

        res.update_current_width()?;

        Ok(res)
    }

    fn update_current_width(&mut self) -> Result<(), String> {
        let operand_line = self.lines.last().expect("No lines!");

        // None = end of line
        let word_width = operand_line
            .chars()
            .skip(2)
            .take_while(|c| c.is_whitespace())
            .count();

        if word_width == 0 {
            return Err(String::from("End of line reached"));
        }

        self.current_width = word_width;

        Ok(())
    }

    fn current(&self) -> Vec<&'a str> {
        let mut current_values = Vec::new();

        for line in &self.lines {
            current_values.push(&line[1..self.current_width + 1]);
        }

        current_values
    }

    fn advance(&mut self) -> Result<(), String> {
        for line in &mut self.lines {
            *line = &line.split_at(self.current_width).1[1..];
        }

        self.update_current_width()?;

        Ok(())
    }
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = args().collect();
    let mut raw_input = fs::read_to_string(&args[1]).expect("Input not found! {&args[1]}");

    raw_input = raw_input.replace("\r\n", " \r\n ");
    raw_input = format!(" {raw_input}");
    raw_input = raw_input
        .chars()
        .rev()
        .collect::<String>()
        .replacen("\n", "\n ", 1)
        .chars()
        .rev()
        .collect();

    let mut input = Input::build(raw_input.lines().collect()).expect("Failed to build input");

    let mut total = 0;
    loop {
        let current_values = input.current();

        total += calculate(
            &current_values[..current_values.len() - 1],
            &current_values[current_values.len() - 1],
        );

        let adv = input.advance();
        match adv {
            Ok(_) => (),
            Err(_) => break,
        }
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("Total: {total}");
}

fn calculate(operands: &[&str], operator: &str) -> u64 {
    let operation: fn(u64, u64) -> u64;
    operation = match operator.trim() {
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

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_input() {
        let test_cases = vec![(
            Input::build(vec![
                " 123 328  51 64  ",
                "  45 64  387 23  ",
                "   6 98  215 314 ",
                " *   +   *   +   ",
            ])
            .expect("building Input failed"),
            [
                ["123", " 45", "  6", "*  "],
                ["328", "64 ", "98 ", "+  "],
                [" 51", "387", "215", "*  "],
                ["64 ", "23 ", "314", "+  "],
            ],
        )];

        for (mut input, expected_results) in test_cases {
            for expected_result in expected_results {
                {
                    let result = input.current();
                    assert_eq!(result.len(), expected_result.len());
                    for i in 0..expected_result.len() {
                        assert_eq!(expected_result[i], result[i]);
                    }
                }
                let adv = input.advance();
                match adv {
                    Ok(_) => (),
                    Err(_) => continue,
                }
            }
        }
    }
}
