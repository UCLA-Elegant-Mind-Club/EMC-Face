%% Main folder selection (don't need to change):
% mainFolder = "C:\Users\elega\OneDrive\Documents\GitHub\RSST\Eye tracking\Data";
mainFolder = "C:\Users\elega\OneDrive\Documents\GitHub\RSST\Eye tracking\NettaPresentation";
% copy your data to this directory for analysis

%% Analysis:
folder = dir(mainFolder);
eye = [];
data = [];

for i = 1:length(folder)
    if folder(i).name(end) ~= '.' && folder(i).isdir
        subfolderName = fullfile(mainFolder, folder(i).name);
        subfolder = dir(subfolderName);
        for j = 1:length(subfolder)
            if subfolder(j).name(end) == 'v'
                if subfolder(j).name(end-9:end) == 'Covert.csv'
                    data = [data ; readtable(fullfile(subfolderName, subfolder(j).name))];
                else
                    eye = [eye ; readtable(fullfile(subfolderName, subfolder(j).name))];
                end
            end
        end
    end
end

writetable(data, fullfile(mainFolder, "reaction_time.csv"))
writetable(eye, fullfile(mainFolder, "eye_tracking.csv"))

start_time = data{:,end}; % + 1.5 for tobii 5 ?
resp_time = data{:,end-1};
end_time = resp_time + start_time;


figure()
colors = [[0, 0.4470, 0.7410]	        ;      
          	[0.8500, 0.3250, 0.0980]	;          
          	[0.9290, 0.6940, 0.1250]    ;	          	
          	[0.4940, 0.1840, 0.5560]	;          	
          	[0.4660, 0.6740, 0.1880]	;         
          	[0.3010, 0.7450, 0.9330]	;
            [0.6350, 0.0780, 0.1840]	; 
            [0.25, 0.25, 0.25]
            ];
angles = [0, 15, 30];   % issue: no 45 degree can be displayed on screen

hold on
for t = 1:length(start_time)-1
    c =  find(angles==data{t,3});
    eye_sub = eye( eye{:,1}<end_time(t) & eye{:,1}>start_time(t),:);
    plot(eye_sub{:,1} - eye_sub{1,1}, eye_sub{:,2},'-o','Color',colors(c,:),'MarkerSize',1.2)
end
xlabel("Time (s)")
ylabel("Eccentricity (deg)")
% xlim([0.35,0.6])
xlim([0.2,1.5])
ecc_to_dis = [3840/2-tand(flip(angles(2:end)))*3840/2 3840/2+tand(angles)*3840/2];
yticks(ecc_to_dis)
ylim([0,3840])
yticklabels([-flip(angles(2:end)) angles])

% avg_resp is average response time.
avg_resp = cell(1,length(angles)*2-1);
for i = 1:size(data,1)
    j = find(data{i,3}==angles);
    if data{i,2}==0
        j = j + length(angles) - 1;
    else
        j = length(angles) - j + 1;
    end
    avg_resp{1,j}{end+1} = data{i,4};
end
for i = 1:size(avg_resp,2)
    avg_resp{1,i} = mean(cell2mat(avg_resp{1,i}));
end
for i = 1:size(avg_resp,2)
    scatter(avg_resp{1,i}, ecc_to_dis(i),15,'black','filled')
end

hold off
