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
    let start_str = chunks.get(0);
    let end_str = chunks.get(1);

    let start: u64;
    let end: u64;

    match start_str {
        None => panic!("start of range not valid: {}", range_str),
        Some(s) => {
            let temp_start = s.trim().parse::<u64>();
            match temp_start {
                Err(error) => panic!("Parsing went wrong! start: {s} {error}"),
                Ok(s) => {
                    start = s;
                    ()
                }
            }
            ()
        }
    }

    match end_str {
        None => panic!("end of range not valid: {}", range_str),
        Some(e) => {
            let temp_end = e.trim().parse::<u64>();
            match temp_end {
                Err(error) => panic!("Parsing went wrong! end: {e} {error}"),
                Ok(e) => {
                    end = e;
                    ()
                }
            }
            ()
        }
    }

    Range {
        start: start,
        end: end,
    }
}
