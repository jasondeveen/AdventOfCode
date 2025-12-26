use std::{env, fs, time::SystemTime};

static PART2: bool = true;

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = env::args().collect();

    let contents_raw = fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let mut total_joltage: u64 = 0;

    for line in contents_raw.split_whitespace() {
        total_joltage += get_max_joltage(line, if PART2 { 12 } else { 2 });
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("Total joltage: {total_joltage}");
}

fn get_max_joltage(line: &str, battery_length: u8) -> u64 {
    let mut result: u64 = 0;
    let _line_length: u8 = line.len() as u8;
    let mut window_size = line.len() as u8 - battery_length + 1;
    let mut amount_skipped: u8 = 0;

    for i in 0..battery_length {
        let offset = i + amount_skipped;
        let (index, largest_number_in_window) =
            get_largest_number(&line[offset as usize..(offset + window_size) as usize]);

        result +=
            largest_number_in_window as u64 * (10 as u64).pow((battery_length - i - 1) as u32);
        amount_skipped += index as u8;
        window_size -= index as u8;
    }

    result
}

fn get_largest_number(line: &str) -> (usize, u8) {
    let mut largest_encountered: u8 = 0;
    let mut index_largest_encountered = 0;

    for (i, c) in line.chars().enumerate() {
        let num = c.to_digit(10).unwrap() as u8;
        if num > largest_encountered {
            largest_encountered = num;
            index_largest_encountered = i;
        }
    }

    (index_largest_encountered, largest_encountered)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_max_joltage_p1() {
        let test_cases: [(&str, u64); 4] = [
            ("987654321111111", 98),
            ("811111111111119", 89),
            ("234234234234278", 78),
            ("818181911112111", 92),
        ];

        for test_case in test_cases {
            assert_eq!(
                get_max_joltage(test_case.0, 2),
                test_case.1,
                "FAIL: expected {}, string = {}",
                test_case.1,
                test_case.0
            );
        }
    }

    #[test]
    fn test_get_max_joltage_p2() {
        let test_cases: [(&str, u64); 4] = [
            ("987654321111111", 987654321111),
            ("811111111111119", 811111111119),
            ("234234234234278", 434234234278),
            ("818181911112111", 888911112111),
        ];

        for test_case in test_cases {
            assert_eq!(
                get_max_joltage(test_case.0, 12),
                test_case.1,
                "FAIL: expected {}, string = {}",
                test_case.1,
                test_case.0
            );
        }
    }
}
