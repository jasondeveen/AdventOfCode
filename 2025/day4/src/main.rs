use std::{env, fs, time::SystemTime};

#[derive(Debug)]
struct Point {
    x: usize,
    y: usize,
}
fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = env::args().collect();

    let contents_raw = fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let mut lines: Vec<String> = contents_raw
        .split_whitespace()
        .map(|x| x.to_string())
        .collect();

    let width = lines[0].len();
    let height = lines.len();

    let modifiers: Vec<i32> = vec![-1, 0, 1];

    let mut number_of_accessible_rolls: u32 = 0;
    let mut rolls_to_move: Vec<Point> = vec![];
    loop {
        for (row, line) in lines.iter().enumerate() {
            for (col, c) in line.chars().enumerate() {
                if c != '@' {
                    continue;
                }

                let mut papers_around_me = 0;
                for col_modifier in &modifiers {
                    for row_modifier in &modifiers {
                        if *col_modifier == 0 && *row_modifier == 0 {
                            continue;
                        }
                        if get_symbol_at_coor(
                            row as i32 + row_modifier,
                            col as i32 + col_modifier,
                            &lines,
                            width,
                            height,
                        ) == '@'
                        {
                            papers_around_me += 1;
                        }
                    }
                }

                if papers_around_me < 4 {
                    number_of_accessible_rolls += 1;
                    rolls_to_move.push(Point { x: col, y: row });
                }
            }
        }

        if rolls_to_move.len() == 0 {
            break;
        }

        for roll_to_move in &rolls_to_move {
            lines
                .get_mut(roll_to_move.y)
                .unwrap()
                .replace_range(roll_to_move.x..roll_to_move.x + 1, ".");
        }

        rolls_to_move.clear();
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("Number of accessible rolls: {number_of_accessible_rolls}");
}

fn get_symbol_at_coor(vert: i32, hor: i32, map: &Vec<String>, width: usize, height: usize) -> char {
    if vert < 0 || hor < 0 {
        return ' ';
    }

    if vert as usize >= height || hor as usize >= width {
        return ' ';
    }

    map[vert as usize].as_bytes()[hor as usize] as char
}
