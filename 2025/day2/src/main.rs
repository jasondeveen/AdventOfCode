use std::time::SystemTime;
use std::{env, fs};

#[derive(Debug)]
struct Range {
    start: u64,
    end: u64,
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = env::args().collect();

    let contents_raw: String =
        fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let ranges = contents_raw
        .split(',')
        .map(|raw_range| parse_range(raw_range));

    let mut sum_of_invalids: u64 = 0;
    for range in ranges {
        sum_of_invalids += detect_invalids(range);
    }

    println!("sum of invalids: {sum_of_invalids}");
    println!("{:?}", now.elapsed().unwrap());
}

fn detect_invalids(range: Range) -> u64 {
    let mut sum = 0;

    for num in range.start..range.end + 1 {
        if is_number_invalid(num) {
            sum += num;
        }
    }

    sum
}

fn is_number_invalid(num: u64) -> bool {
    let s = num.to_string();
    if s.len() % 2 == 0 {
        let first_half = &s[..s.len() / 2];
        let second_half = &s[s.len() / 2..];

        if first_half == second_half {
            return true;
        }
    }

    return false;
}

fn parse_range(range_str: &str) -> Range {
    let chunks: Vec<&str> = range_str.split('-').collect();
    let start = chunks
        .get(0)
        .expect("No second part of the range (start): {range_str}")
        .trim()
        .parse::<u64>()
        .expect("start part of range is not a number: {range_str}");
    let end = chunks
        .get(1)
        .expect("No second part of the range (end): {range_str}")
        .trim()
        .parse::<u64>()
        .expect("End part of range is not a number: {range_str}");

    Range {
        start: start,
        end: end,
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_is_number_invalid() {
        let test_cases = vec![
            11, 22, 111, 1010, 222222, 824824824, 38593859, 1188511885, 112112,
        ];

        for test_number in test_cases {
            assert!(is_number_invalid(test_number), "FAIL: {test_number}");
        }
    }
}
