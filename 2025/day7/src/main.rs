use std::{fs, time::SystemTime};

fn main() {
    let now = SystemTime::now();

    let args: Vec<String> = std::env::args().collect();
    let raw_input = fs::read_to_string(&args[1]).expect("input couldnt be read.");

    let mut lines = raw_input.lines();
    let start_pos = lines
        .next()
        .expect("Cant read first line!")
        .chars()
        .enumerate()
        .find(|(_i, c)| *c == 'S')
        .expect("Couldnt find starting position")
        .0;

    let total_breaks = 0;
    let mut incoming_beams = vec![start_pos as u8];
    for line in lines {
        let outgoing_beams = step_line(&incoming_beams, line);
        if outgoing_beams.len() > 0 {
            incoming_beams.clear();
            incoming_beams.clone_from_slice(&outgoing_beams);
        }
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("Total beam breaks = {}", total_breaks);
}

fn step_line(incoming_beams: &Vec<u8>, line: &str) -> Vec<u8> {
    let spike_positions: Vec<u8> = line
        .chars()
        .enumerate()
        .filter(|(_i, c)| *c == '^')
        .map(|(i, _c)| i as u8)
        .collect();

    let mut outgoing_beams: Vec<u8> = incoming_beams.clone();

    for incoming_beam in incoming_beams {
        if spike_positions.contains(incoming_beam) {
            if *incoming_beam > 0 {
                outgoing_beams.push((incoming_beam - 1) as u8);
            }
            if *incoming_beam < (line.len() - 1) as u8 {
                outgoing_beams.push((incoming_beam + 1) as u8);
            }
        }
    }

    outgoing_beams.sort();
    outgoing_beams.dedup(); // only dedups dups when list is sorted!

    for spike_position in &spike_positions {
        let to_remove = outgoing_beams
            .iter()
            .enumerate()
            .find(|(_i, b)| *b == spike_position)
            .map(|(i, _b)| i);

        if let Some(idx) = to_remove {
            outgoing_beams.remove(idx);
        }
    }

    outgoing_beams
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_step_line() {
        let test_cases = [
            (vec![7], ".......^.......", vec![6, 8]),
            (vec![6, 8], "......^.^......", vec![5, 7, 9]),
            (vec![5, 7, 9], "...............", vec![5, 7, 9]),
            (vec![0, 1, 3], "^^.^...", vec![2, 4]),
            (vec![0, 1, 3, 4, 7, 8, 9], "..^^^....^", vec![0, 1, 5, 7, 8]),
            (vec![0, 1, 2, 3, 4, 5, 6, 7, 8, 9], "^^^^^^^^^^", vec![]),
            (vec![0, 1, 2, 4, 5, 6, 7, 8, 9], "^^^.^^^^^^", vec![3]),
        ];

        for (incoming_beams, line, expected_result) in test_cases {
            let result = step_line(&incoming_beams, line);
            assert_eq!(result, expected_result);
        }
    }
}
