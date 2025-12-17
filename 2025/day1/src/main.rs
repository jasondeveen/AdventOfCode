use std::env;
use std::fs;

static PART2: bool = true;

fn main() {
    let args: Vec<String> = env::args().collect();

    let contents_raw: String =
        fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let contents = contents_raw.split_whitespace();

    let mut safe = Safe { current_pos: 50 };
    let mut zero_hits = 0;
    for line in contents.into_iter() {
        let zero_passes = safe.apply_move(translate_move(line));
        if PART2 {
            zero_hits += zero_passes;
        }
        zero_hits += if safe.current_pos == 0 { 1 } else { 0 };
    }

    println!("Zero hits: {zero_hits}")
}

#[derive(Debug)]
struct Safe {
    current_pos: i16, // between 0 and 99
}

/// Returns the amount of times the dial went past 0
impl Safe {
    fn apply_move(&mut self, rotation: i32) -> u32 {
        let theoretical_next_pos = self.current_pos as i32 + rotation;
        let full_turns = rotation.abs() as u32 / 100;
        let rotation_trunc: i16 = (rotation % 100) as i16;
        let cross_zero = if (self.current_pos + rotation_trunc < 0 && self.current_pos != 0)
            || self.current_pos + rotation_trunc > 100
        {
            1
        } else {
            0
        };

        dbg!(self.current_pos);
        dbg!(theoretical_next_pos);
        let theoretical_next_pos_trunc: i16 = (theoretical_next_pos % 100) as i16;
        dbg!(theoretical_next_pos_trunc);
        self.current_pos = if theoretical_next_pos_trunc < 0 {
            100 - theoretical_next_pos_trunc.abs()
        } else if theoretical_next_pos_trunc >= 100 {
            theoretical_next_pos_trunc - 100
        } else {
            theoretical_next_pos_trunc
        };

        dbg!(self.current_pos);
        dbg!(full_turns);
        dbg!(cross_zero);
        full_turns + cross_zero
    }
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

    dbg!(direction * distance)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn run_safe_apply_move_tests() {
        let test_cases = vec![
            (Safe { current_pos: 50 }, 15, 65, 0),
            (Safe { current_pos: 50 }, 65, 15, 1),
            (Safe { current_pos: 50 }, -15, 35, 0),
            (Safe { current_pos: 50 }, -65, 85, 1),
        ];

        for (mut safe, rotation, expected_end, expected_zero_passes) in test_cases {
            let real_zero_passes = safe.apply_move(rotation);

            assert_eq!(
                real_zero_passes, expected_zero_passes,
                "FAIL: {safe:?}, {rotation}, {expected_end}, {expected_zero_passes}"
            );
            assert_eq!(
                safe.current_pos, expected_end,
                "FAIL: {safe:?}, {rotation}, {expected_end}, {expected_zero_passes}"
            );
        }
    }
}
